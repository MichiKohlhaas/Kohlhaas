namespace Kohlhaas.DB;

public class Worker(IBackgroundTaskQueue taskQueue, ILogger<Worker> logger) : BackgroundService
{
    

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("""
                              {Name} is running.
                              Tap W to add a work item to the 
                              background queue.
                              """,
            nameof(Worker));

        return ProcessTaskQueueAsync(stoppingToken);
    }
    
    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Func<CancellationToken, ValueTask>? workItem =
                    await taskQueue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            $"{nameof(Worker)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}