using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Services;
using OdeonFlightInvoiceCase.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OdeonFlightInvoiceCase.Infrastructure.Repositories;
using OdeonFlightInvoiceCase.Application.Services;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/invoice-processor-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Register services
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        
        // Configure MailSettings
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        
        // Add DbContext
        var connectionString = configuration.GetConnectionString("PostgreSql");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Could not find the connection string 'PostgreSql' in the configuration.");
        }

        Log.Information($"Using connection string: {connectionString}");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register repositories
        services.AddScoped<IReservationRepository, ReservationRepository>();
        
        // Register services
        services.AddScoped<IInvoiceParser, PdfInvoiceParser>();
        services.AddScoped<IMailService, SmtpMailService>();
        services.AddScoped<IInvoiceMatchService, InvoiceMatchService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(Program).Assembly);

        // Register the main service
        services.AddHostedService<InvoiceProcessingService>();
    });

var host = builder.Build();

try
{
    Log.Information("Starting invoice processing application");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
