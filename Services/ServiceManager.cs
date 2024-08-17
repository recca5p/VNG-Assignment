using Amazon.SimpleEmail;
using AutoMapper;
using Contract.Models;
using Domain.RepositoriyInterfaces;
using Microsoft.Extensions.Options;
using Services.Abstractions;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _lazyUserService;
    public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper, appSettings));
    }
    
    public IUserService UserService => _lazyUserService.Value;
}