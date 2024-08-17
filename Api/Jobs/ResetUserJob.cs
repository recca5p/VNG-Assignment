
using Quartz;

namespace Services.Jobs;

[DisallowConcurrentExecution]
public class ResetUserJob : IJob
{
    private readonly ILogger<ResetUserJob> _logger;
    
    public ResetUserJob(ILogger<ResetUserJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Test");
        
        await Task.CompletedTask;
    }
}