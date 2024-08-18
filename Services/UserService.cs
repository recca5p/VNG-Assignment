using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon;
using Amazon.SimpleEmail;
using AutoMapper;
using Contract.DTOs.Users;
using Contract.Enum;
using Contract.Exceptions.Users;
using Contract.Helpers;
using Contract.Models;
using Domain.Entities;
using Domain.RepositoriyInterfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;

namespace Services
{
    public sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            var sesClient = new AmazonSimpleEmailServiceClient(_appSettings.AWS.AccessKey, _appSettings.AWS.SecretKey, RegionEndpoint.APSoutheast1);
            EmailHelper.Initialize(sesClient, _appSettings.AWS.SimpleEmail.DefaultDomain);
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<User> users = await _repositoryManager.UserRepository.GetAllAsync(cancellationToken);

            IEnumerable<UserModel> result = _mapper.Map<IEnumerable<User>, IEnumerable<UserModel>>(users);
            
            return result;
        }

        public async Task<UserModel> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            User user = await _repositoryManager.UserRepository.GetByIdAsync(userId, cancellationToken);
        
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            UserModel result = _mapper.Map<User, UserModel>(user);
        
            return result;
        }
        
        public async Task<UserModel> CreateAsync(UserCreateRequest userRequest, CancellationToken cancellationToken = default)
        {
            userRequest.Password = PasswordHashHelper.HashPassword(userRequest.Password);

            User existUser = await _repositoryManager.UserRepository.GetByEmailAsync(userRequest.Email);

            if (existUser != null)
                throw new UserExistException(userRequest.Email);
            
            User user = _mapper.Map<UserCreateRequest, User>(userRequest);
            
            user.Id = Guid.NewGuid();
        
            _repositoryManager.UserRepository.Insert(user);
        
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            
            string token = TokenService.GenerateToken(user.Id, user.Email, _appSettings.JwtSettings.Key, _appSettings.JwtSettings.ExpiryMinutes);
            
            string verificationLink = $"{_appSettings.ApiHost}api/auth/verify-email?token={token}";

            await EmailHelper.SendEmailAsync(userRequest.Email, "Verify your email", $"Please verify your email by clicking <a href=\"{verificationLink}\">here</a>.");
            
            return _mapper.Map<User, UserModel>(user);
        }
        
        public async Task UpdateAsync(Guid userId, UserUpdateRequest userRequest, CancellationToken cancellationToken = default)
        {
            User user = await _repositoryManager.UserRepository.GetByIdAsync(userId);
        
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            user.Password = PasswordHashHelper.HashPassword(user.Password);
            
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _repositoryManager.UserRepository.GetByIdAsync(userId, cancellationToken);
        
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }
        
            _repositoryManager.UserRepository.Remove(user);
        
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }


        public async Task<(string, Guid)> AuthenticateAsync(UserSignInRequest request,
            CancellationToken cancellationToken = default)
        {
            request.Password = PasswordHashHelper.HashPassword(request.Password);
            
            User user = _mapper.Map<UserSignInRequest, User>(request);

            User userExist = await _repositoryManager.UserRepository.GetUserByEmailAndPassword(user, cancellationToken);

            if (userExist is null)
                throw new UserEmailOrPasswordException();

            if (user.Status is UserStatus.Verified)
                throw new UserNeededToVerifyException(user.Email);
            
            if (user.Status is UserStatus.RequiredChangePwd || user.Status is UserStatus.IsSendChangeEmail)
                throw new UserNeededToChangePasswordException(user.Email);

            string token = TokenService.GenerateToken(userExist.Id, userExist.Email, _appSettings.JwtSettings.Key, _appSettings.JwtSettings.ExpiryMinutes);

            return (token, userExist.Id);
        }
        public async Task<bool> ValidateUser(string token)
        {
            var isValid = TokenService.ValidateToken(token, _appSettings.JwtSettings.Key);

            if (!isValid)
                throw new UnauthorizedAccessException("Token is not valid");

            Guid userId = TokenService.GetUserIdFromToken(token, _appSettings.JwtSettings.Key);

            User user = await _repositoryManager.UserRepository.GetByIdAsync(userId);

            user.Status = UserStatus.Verified;
            user.UpdatedTime = DateTime.UtcNow;

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return isValid;
        }

        public async Task UpdateUsersStatusNeededToBeResetPassword()
        {
            IEnumerable<User> users =
                await _repositoryManager.UserRepository.GetUsersNeededToBeResetPassword(_appSettings
                    .ResetPasswordTimeInSeconds);
            
            foreach (User user in users)
            {
                user.Status = UserStatus.RequiredChangePwd;
                user.UpdatedTime = DateTime.UtcNow;
            }

            await _repositoryManager.UnitOfWork.SaveChangesAsync();
        }
        
        public async Task SendUsersNeededChangePwd()
        {
            IEnumerable<User> users =
                await _repositoryManager.UserRepository.GetUsersNeededToBeResetPasswordEMail();
            
            foreach (User user in users)
            {
                string emailSubject = "Reset your password";
                string emailBody = $@"
                    Please reset your password by clicking
                            <br/><br/>
                            Note: This is just a mock email, and there is no actual web form to fill out. 
                        The link provided will not work as there is no front-end implemented for this feature.
                            <br/><br/>
                            If you did not request a password reset, please ignore this email.";

                await EmailHelper.SendEmailAsync(user.Email, emailSubject, emailBody);   
                user.Status = UserStatus.IsSendChangeEmail;
                user.UpdatedTime = DateTime.UtcNow;
            }

            await _repositoryManager.UnitOfWork.SaveChangesAsync();
        }
    }
}
