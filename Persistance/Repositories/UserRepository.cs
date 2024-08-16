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
}