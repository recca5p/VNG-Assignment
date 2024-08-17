using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon;
using Amazon.SimpleEmail;
using AutoMapper;
using Contract.DTOs.Users;
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
        
            _repositoryManager.UserRepository.Insert(user);
        
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            await EmailHelper.SendEmailAsync(userRequest.Email, "Verify email", "Please verify the email");
            
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


        public async Task<string> AuthenticateAsync(UserSignInRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = _mapper.Map<UserSignInRequest, User>(request);

            User userExist = await _repositoryManager.UserRepository.GetUserByEmailAndPassword(user, cancellationToken);

            if (userExist is null)
                throw new UserEmailOrPasswordException();

            string token = TokenService.GenerateToken(userExist.Id, userExist.Email, _appSettings.JwtSettings.Key, _appSettings.JwtSettings.ExpiryMinutes);

            return token;
        }
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSettings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
    }
}
