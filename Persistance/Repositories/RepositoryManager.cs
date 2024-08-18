using Domain.RepositoriyInterfaces;

namespace Persistance.Repositories;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IUserRepository> _lazyUserRepository;
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;
    private readonly Lazy<IBookRepository> _lazyBookRepository;
    
    public RepositoryManager(IRepositoryDbContext repositoryDbContext)
    {
        _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryDbContext));
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(repositoryDbContext));
        _lazyBookRepository = new Lazy<IBookRepository>(() => new BookRepository(repositoryDbContext));
    }
    
    public IUserRepository UserRepository => _lazyUserRepository.Value;
    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
    public IBookRepository BookRepository => _lazyBookRepository.Value;
}