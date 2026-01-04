using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebPortal.Infrastructure.Persistence;

namespace WebPortal.Infrastructure.Monitoring;

public sealed class SqlConnectionHealthCheck : IHealthCheck
{
    private readonly WebPortalDbContext _db;

    public SqlConnectionHealthCheck(WebPortalDbContext db) => _db = db;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ok = await _db.Database.CanConnectAsync(cancellationToken);
            return ok
                ? HealthCheckResult.Healthy("SQL Server reachable")
                : HealthCheckResult.Unhealthy("SQL Server not reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server check failed", ex);
        }
    }
}
