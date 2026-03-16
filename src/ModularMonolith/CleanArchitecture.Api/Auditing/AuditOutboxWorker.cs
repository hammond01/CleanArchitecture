namespace CleanArchitecture.Api.Auditing;

public sealed class AuditOutboxWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditOutboxWorker> _logger;

    public AuditOutboxWorker(
        IServiceProvider serviceProvider,
        ILogger<AuditOutboxWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IAuditOutboxProcessor>();
                var processedCount = await processor.ProcessPendingMessagesAsync(100, stoppingToken);
                var delay = processedCount > 0
                    ? TimeSpan.FromMilliseconds(250)
                    : TimeSpan.FromSeconds(2);

                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Audit outbox worker failed");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
