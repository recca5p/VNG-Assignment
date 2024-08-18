using Quartz;
using Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Services.Jobs;

[DisallowConcurrentExecution]
public class BookCacheUpdaterServiceJob : IJob
{
    private readonly ILogger<BookCacheUpdaterServiceJob> _logger;
    private readonly IServiceManager _serviceManager;
    
    public BookCacheUpdaterServiceJob(ILogger<BookCacheUpdaterServiceJob> logger, IServiceManager serviceManager)
    {
        _logger = logger;
        _serviceManager = serviceManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Book cache update job started.");

        try
        {
            await _serviceManager.BookService.RefreshCache();

            _logger.LogInformation("Book cache update completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the book cache.");
        }

        _logger.LogInformation("Book cache update job ended.");
    }
}