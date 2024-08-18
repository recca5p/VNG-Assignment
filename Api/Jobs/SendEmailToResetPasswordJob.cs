using Quartz;
using Services.Abstractions;

namespace Services.Jobs;

[DisallowConcurrentExecution]
public class SendEmailToResetPasswordJob : IJob
{
    private readonly ILogger<SendEmailToResetPasswordJob> _logger;
    private readonly IServiceManager _serviceManager;
    
    public SendEmailToResetPasswordJob(ILogger<SendEmailToResetPasswordJob> logger,
        IServiceManager servicemanager)
    {
        _logger = logger;
        _serviceManager = servicemanager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("BEGIN");

        await _serviceManager.UserService.SendUsersNeededChangePwd();
        
        _logger.LogInformation("END");

        await Task.CompletedTask;
    }
}