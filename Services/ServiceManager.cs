using AutoMapper;
using Domain.RepositoriyInterfaces;
using Services.Abstractions;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _lazyUserService;
    
    public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
    {
        _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper));
    }
    
    public IUserService UserService => _lazyUserService.Value;
}