using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OdeonFlightInvoiceCase.Domain.Interfaces;

namespace OdeonFlightInvoiceCase.Application.Services;

public class InvoiceProcessingService : IHostedService
{
    private readonly ILogger<InvoiceProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Task? _executingTask;
    private CancellationTokenSource? _stoppingCts;

    public InvoiceProcessingService(
        ILogger<InvoiceProcessingService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            _stoppingCts?.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var invoiceMatchService = scope.ServiceProvider.GetRequiredService<IInvoiceMatchService>();
            var invoiceParser = scope.ServiceProvider.GetRequiredService<IInvoiceParser>();

            // TODO: Implement file watching logic here
            // For now, we'll just process a single file
            var filePath = "C:/Users/Erkan/Downloads/Invoice_10407.PDF"; // Replace with actual file path
            await invoiceMatchService.ProcessInvoiceAsync(filePath, invoiceParser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing invoice");
        }
    }
} 