using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Services.Models;

namespace OdeonFlightInvoiceCase.Application.Services;

public class InvoiceProcessingService : IHostedService
{
    private readonly ILogger<InvoiceProcessingService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly FileSettings _fileSettings;
    private Task? _executingTask;
    private CancellationTokenSource? _stoppingCts;

    public InvoiceProcessingService(
        ILogger<InvoiceProcessingService> logger,
        IServiceProvider serviceProvider,
        IOptions<FileSettings> fileSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _fileSettings = fileSettings.Value;
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

            if (!File.Exists(_fileSettings.InvoiceFilePath))
            {
                _logger.LogError($"Invoice file not found at path: {_fileSettings.InvoiceFilePath}");
                return;
            }

            _logger.LogInformation($"Processing invoice file: {_fileSettings.InvoiceFilePath}");
            await invoiceMatchService.ProcessInvoiceAsync(_fileSettings.InvoiceFilePath, invoiceParser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing invoice");
        }
    }
} 