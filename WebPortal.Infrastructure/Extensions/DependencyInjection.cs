using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using WebPortal.Application.Abstractions.Caching;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Application.Services;
using WebPortal.Infrastructure.Caching;
using WebPortal.Infrastructure.Monitoring;
using WebPortal.Infrastructure.Persistence;
using WebPortal.Infrastructure.Repositories;

namespace WebPortal.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddWebPortalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConn = configuration.GetConnectionString("WebPortalDb")
                    ?? configuration["ConnectionStrings:WebPortalDb"];

        if (string.IsNullOrWhiteSpace(dbConn))
            throw new InvalidOperationException("Missing connection string. Please set ConnectionStrings:WebPortalDb.");

        services.AddDbContext<WebPortalDbContext>(options =>
        {
            options.UseSqlServer(dbConn);
        });

        services.AddMemoryCache();

        // Optional Redis - only enabled when ConnectionStrings:Redis is provided.
        var redisConn = configuration.GetConnectionString("Redis")
                        ?? configuration["ConnectionStrings:Redis"];

        if (!string.IsNullOrWhiteSpace(redisConn))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConn));
        }

        services.AddScoped<ICacheService, HybridCacheService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ILinkRepository, LinkRepository>();
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

            services.AddScoped<ISystemMonitorService, SystemMonitorService>();

        return services;
    }
}
