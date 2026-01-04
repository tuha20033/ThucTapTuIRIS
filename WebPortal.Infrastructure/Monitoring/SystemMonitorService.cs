using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WebPortal.Application.Models;
using WebPortal.Application.Services;
using WebPortal.Infrastructure.Persistence;

namespace WebPortal.Infrastructure.Monitoring;

public sealed class SystemMonitorService : ISystemMonitorService
{
    private static readonly DateTime StartUtc = DateTime.UtcNow;

    private readonly WebPortalDbContext _db;
    private readonly IDatabase? _redis;

    public SystemMonitorService(WebPortalDbContext db, IServiceProvider services)
    {
        _db = db;
        _redis = services.GetService<IConnectionMultiplexer>()?.GetDatabase();
    }

    public async Task<SystemStatusModel> GetStatusAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var dbOk = await _db.Database.CanConnectAsync(ct);

        bool redisOk = false;
        if (_redis is not null)
        {
            try
            {
                await _redis.PingAsync();
                redisOk = true;
            }
            catch
            {
                redisOk = false;
            }
        }

        // Counts (fast enough for a small monitoring page; avoid on every request)
        var users = await _db.Users.CountAsync(ct);
        var categories = await _db.Categories.CountAsync(ct);
        var links = await _db.Links.CountAsync(ct);
        var favs = await _db.FavoriteLinks.CountAsync(ct);

        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
                      ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                      ?? "unknown";

        return new SystemStatusModel(
            UtcNow: now,
            Uptime: now - StartUtc,
            Version: version,
            DatabaseOk: dbOk,
            RedisOk: redisOk,
            Users: users,
            Categories: categories,
            Links: links,
            FavoriteLinks: favs
        );
    }
}
