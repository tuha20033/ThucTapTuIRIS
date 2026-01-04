using Microsoft.EntityFrameworkCore;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence;

public sealed class WebPortalDbContext : DbContext
{
    public WebPortalDbContext(DbContextOptions<WebPortalDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Link> Links => Set<Link>();
    public DbSet<FavoriteLink> FavoriteLinks => Set<FavoriteLink>();
    public DbSet<LinkSequence> LinkSequence => Set<LinkSequence>();
    public DbSet<CategorySequence> CategorySequence => Set<CategorySequence>();
    public DbSet<UserCategoryState> UserCategoryStates => Set<UserCategoryState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebPortalDbContext).Assembly);
    }
}
