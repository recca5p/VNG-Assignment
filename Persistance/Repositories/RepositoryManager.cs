using Domain.RepositoriyInterfaces;

namespace Persistance.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IUserRepository> _lazyUserRepository;
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;
    
    public RepositoryManager(IRepositoryDbContext repositoryDbContext)
    {
        _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryDbContext));
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(repositoryDbContext));
    }
    
    public IUserRepository UserRepository => _lazyUserRepository.Value;
    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
}