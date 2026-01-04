using Microsoft.Extensions.DependencyInjection;
using WebPortal.Application.Services;

namespace WebPortal.Application.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddWebPortalApplication(this IServiceCollection services)
    {
        services.AddScoped<IPortalService, PortalService>();
        services.AddScoped<ICatalogAdminService, CatalogAdminService>();
        return services;
    }
}
