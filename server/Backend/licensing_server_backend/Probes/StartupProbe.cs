using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

public class StartupHealthCheck : IHealthCheck
{
    private bool _startupComplete = false;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Simulate startup logic
        if (_startupComplete)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Startup is complete."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Startup is not yet complete."));
    }

    // Simulate completing startup (could be triggered externally)
    public void MarkStartupComplete()
    {
        _startupComplete = true;
    }
}