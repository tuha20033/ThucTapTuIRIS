using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace WebPortal.Infrastructure.Monitoring;

public sealed class RedisConnectionHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer? _mux;

    public RedisConnectionHealthCheck(IConnectionMultiplexer? mux = null)
    {
        _mux = mux;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_mux is null)
            return HealthCheckResult.Healthy("Redis not configured");

        try
        {
            var db = _mux.GetDatabase();
            await db.PingAsync().ConfigureAwait(false);
            return HealthCheckResult.Healthy("Redis reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Redis not reachable", ex);
        }
    }
}
