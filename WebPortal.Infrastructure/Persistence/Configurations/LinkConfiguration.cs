using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> b)
    {
        b.ToTable("Links", t =>
        {
            t.HasCheckConstraint("CK_Links_Url_Security", "Url NOT LIKE 'javascript:%' AND Url NOT LIKE 'data:%'");
        });

        b.HasKey(x => x.Id).HasName("PK_Links");

        b.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        b.Property(x => x.Name)
            .HasColumnType("nvarchar(150)")
            .IsRequired();

        b.Property(x => x.Url)
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        b.Property(x => x.Icon).HasColumnType("nvarchar(255)");
        b.Property(x => x.Color).HasColumnType("nvarchar(20)");
        b.Property(x => x.Tags).HasColumnType("nvarchar(255)");

        b.Property(x => x.Priority)
            .HasDefaultValue(0)
            .IsRequired();

        b.Property(x => x.RolePrefix)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.CategoryId).IsRequired();

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        b.HasIndex(x => new { x.CategoryId, x.IsActive })
            .HasDatabaseName("IX_Links_Category_IsActive");

        b.HasIndex(x => x.RolePrefix)
            .HasDatabaseName("IX_Links_RolePrefix");
    }
}
