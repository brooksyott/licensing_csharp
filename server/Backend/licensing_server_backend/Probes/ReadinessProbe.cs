using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

public class ReadinessHealthCheck : IHealthCheck
{
    private bool _dependenciesAvailable = true; // Replace with actual dependency check logic

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_dependenciesAvailable)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Dependencies are available."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Dependencies are unavailable."));
    }
}

