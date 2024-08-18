using Amazon.SimpleEmail;
using AutoMapper;
using Contract.Models;
using Domain.RepositoriyInterfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Services.Abstractions;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _lazyUserService;
    private readonly Lazy<IBookService> _lazyBookService;
    public ServiceManager(IRepositoryManager repositoryManager,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IMemoryCache cache)
    {
        _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager, mapper, appSettings));
        _lazyBookService = new Lazy<IBookService>(() => new BookService(repositoryManager, mapper, appSettings, cache));
    }
    
    public IUserService UserService => _lazyUserService.Value;
    public IBookService BookService => _lazyBookService.Value;
}