namespace Domain.RepositoriyInterfaces;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    IUnitOfWork UnitOfWork { get; }
}