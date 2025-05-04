using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using OdeonFlightInvoiceCase.Domain.Interfaces;
using OdeonFlightInvoiceCase.Infrastructure.Services;
using OdeonFlightInvoiceCase.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OdeonFlightInvoiceCase.Infrastructure.Repositories;
using OdeonFlightInvoiceCase.Application.Services;
using OdeonFlightInvoiceCase.Infrastructure.Services.Models;
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
        
        // Configure settings
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

        // Register repositories
        services.AddScoped<IReservationRepository, ReservationRepository>();
        
        // Register services
        services.AddScoped<IInvoiceParser, PdfInvoiceParser>();
        services.AddScoped<IMailService, SmtpMailService>();
        services.AddScoped<IInvoiceMatchService, InvoiceMatchService>();

        // Register AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
