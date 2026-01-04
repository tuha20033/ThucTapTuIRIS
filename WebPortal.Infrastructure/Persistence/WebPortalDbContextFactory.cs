using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebPortal.Infrastructure.Persistence;

public sealed class WebPortalDbContextFactory : IDesignTimeDbContextFactory<WebPortalDbContext>
{
    public WebPortalDbContext CreateDbContext(string[] args)
    {
        var conn = System.Environment.GetEnvironmentVariable("WEBPORTAL_DB");
        if (string.IsNullOrWhiteSpace(conn))
            conn = "Server=.;Database=WebPortalDB5;Trusted_Connection=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<WebPortalDbContext>()
            .UseSqlServer(conn)
            .Options;

        return new WebPortalDbContext(options);
    }
}
