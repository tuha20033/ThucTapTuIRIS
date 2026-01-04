using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class FavoriteLinkConfiguration : IEntityTypeConfiguration<FavoriteLink>
{
    public void Configure(EntityTypeBuilder<FavoriteLink> b)
    {
        b.ToTable("FavoriteLinks");

        b.HasKey(x => new { x.UserId, x.LinkId })
            .HasName("PK_UserFavoriteLinks");

        b.Property(x => x.UserId)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        b.Property(x => x.PinnedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany(u => u.FavoriteLinks)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_FavoriteLinks_Users")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Link)
            .WithMany(l => l.FavoriteLinks)
            .HasForeignKey(x => x.LinkId)
            .HasConstraintName("FK_FavoriteLinks_Links")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_FavoriteLinks_UserId");
    }
}
