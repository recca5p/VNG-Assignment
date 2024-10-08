
using Quartz;
using Services.Abstractions;

namespace Services.Jobs;

[DisallowConcurrentExecution]
public class ResetUserJob : IJob
{
    private readonly ILogger<ResetUserJob> _logger;
    private readonly IServiceManager _serviceManager;
    
    public ResetUserJob(ILogger<ResetUserJob> logger,
        IServiceManager servicemanager)
    {
        _logger = logger;
        _serviceManager = servicemanager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("BEGIN");

        await _serviceManager.UserService.UpdateUsersStatusNeededToBeResetPassword();
        
        _logger.LogInformation("END");

        await Task.CompletedTask;
    }
}