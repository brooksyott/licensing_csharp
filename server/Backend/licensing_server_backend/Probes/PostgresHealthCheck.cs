using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using System.Threading;
using System.Threading.Tasks;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly string? _connectionString;

    public PostgresHealthCheck(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<HealthCheckResult> CheckHealthAsyncOrig(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Simulate a database health check (replace with actual logic)
        bool databaseIsHealthy = true; // Change to actual database health logic

        return databaseIsHealthy
            ? Task.FromResult(HealthCheckResult.Healthy("Database is reachable."))
            : Task.FromResult(HealthCheckResult.Unhealthy("Database is unreachable."));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new NpgsqlCommand("SELECT 1", connection);
            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result != null && (int)result == 1)
            {
                return HealthCheckResult.Healthy("PostgreSQL is healthy.");
            }

            return HealthCheckResult.Unhealthy("PostgreSQL returned an unexpected result.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"PostgreSQL health check failed: {ex.Message}");
        }
    }
}