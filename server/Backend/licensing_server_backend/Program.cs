using HealthChecks.UI.Client;
using Licensing.Data;
using Licensing.Keys;
using Licensing.Skus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.ComponentModel;
using System.Data.Common;

public class Program
{
    static string? _connectionString = null;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) // Reads configuration from appsettings.json
            .Enrich.FromLogContext()
            //.WriteTo.Console() // Write logs to console
            .CreateLogger();

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();


        builder.Host.UseSerilog(); // Replace the default logger

        _connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

        // Configure services
        ConfigureServices(builder.Services);

        var app = builder.Build();

        // Perform startup initialization
        InitializeApp(app);

        // Configure middleware and endpoints
        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<LicensingContext>(options =>
        {
            options.UseNpgsql(_connectionString);
        });


        // Register custom health checks
        services.AddSingleton<StartupHealthCheck>(); // Singleton for stateful checks
        services.AddHealthChecks()
            .AddCheck<StartupHealthCheck>("Startup")
            .AddCheck<LivenessHealthCheck>("Liveness")
            .AddCheck<ReadinessHealthCheck>("Readiness")
            .AddCheck<PostgresHealthCheck>("Postgres",
                    HealthStatus.Unhealthy,
                    tags: new[] { "db", "postgres" });

        services.AddSingleton(sp => new PostgresHealthCheck(_connectionString)); // Pass the connection string

        services.AddControllers(); // Add support for controllers
        services.AddScoped<ISkuService, SkuService>();
        services.AddScoped<IKeyService, KeyService>();


        // Add Swagger services
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Licensing",
                Version = "v1",
                Description = "Simple framework to generate licenses"
            });
        });
    }

    private static void InitializeApp(WebApplication app)
    {
        // Resolve the startup health check to simulate marking startup as complete
        var startupHealthCheck = app.Services.GetRequiredService<StartupHealthCheck>();
        startupHealthCheck.MarkStartupComplete();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseRouting();

        // Enable Swagger middleware
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Licensing V1 API");
            options.RoutePrefix = "swagger/index.html"; // Serve Swagger UI at the root (http://localhost:<port>/)
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Name == "Liveness" || check.Name == "Postgres",
            // Example of writing the full health check report
            ResponseWriter = WriteCustomHealthCheckResponse
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Name == "Readiness" || check.Name == "Postgres",
            // Do not need to write out the full report
            // Example of writing the full health check report
            ResponseWriter = WriteCustomHealthCheckResponse

        });

        app.MapHealthChecks("/health/startup", new HealthCheckOptions
        {
            Predicate = check => check.Name == "Startup",
            // Do not need to write out the full report
        });

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapControllers();
    }

    private static Task WriteCustomHealthCheckResponse(HttpContext httpContext, HealthReport report)
    {
        httpContext.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description ?? "No description provided"
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds + " ms"
        };

        return httpContext.Response.WriteAsJsonAsync(response);
    }
}
