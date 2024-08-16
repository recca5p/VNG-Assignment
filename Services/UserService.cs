using AutoMapper;
using Contract.DTOs.Users;
using Contract.Exceptions.Users;
using Domain.Entities;
using Domain.RepositoriyInterfaces;
using Services.Abstractions;

namespace Services
{
    public sealed class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
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
            User user = _mapper.Map<UserCreateRequest, User>(userRequest);
        
            _repositoryManager.UserRepository.Insert(user);
        
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        
            return _mapper.Map<User, UserModel>(user);
        }
        
        public async Task UpdateAsync(Guid userId, UserUpdateRequest userRequest, CancellationToken cancellationToken = default)
        {
            User user = await _repositoryManager.UserRepository.GetByIdAsync(userId);
        
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            user.Password = user.Password;
            
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
    }
}
