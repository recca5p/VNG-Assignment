using Contract.Enum;
using Domain.Entities;
using Domain.RepositoriyInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IRepositoryDbContext _dbContext;
    
    public UserRepository(IRepositoryDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Users.ToListAsync(cancellationToken);
    
    public async Task<User> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);
    
    public void Insert(User user) => _dbContext.Users.Add(user);
    
    public void Remove(User user) => _dbContext.Users.Remove(user);

    public async Task<User> GetUserByEmailAndPassword(User user, CancellationToken cancellationToken) =>
        await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email && x.Password == user.Password, cancellationToken);
    
    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<IEnumerable<User>> GetUsersNeededToBeResetPassword(int intervalTimeInSeconds, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.Where(u => u.Status == UserStatus.Verified && u.UpdatedTime.Value.AddSeconds(intervalTimeInSeconds) <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<User>> GetUsersNeededToBeResetPasswordEMail(CancellationToken cancellationToken = default) =>
        await _dbContext.Users.Where(u => u.Status == UserStatus.RequiredChangePwd)
            .ToListAsync(cancellationToken);
}