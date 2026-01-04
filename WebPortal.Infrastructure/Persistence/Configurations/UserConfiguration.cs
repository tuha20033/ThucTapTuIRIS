using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;
using WebPortal.Domain.Enums;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");

        b.HasKey(x => x.Id).HasName("PK_Users");

        b.Property(x => x.Id)
            .HasColumnType("nvarchar(100)")
            .ValueGeneratedNever();

        b.Property(x => x.UserName)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        b.HasAlternateKey(x => x.UserName)
            .HasName("UQ_Users_UserName");

        b.Property(x => x.FullName).HasColumnType("nvarchar(100)");
        b.Property(x => x.Email).HasColumnType("nvarchar(100)");

        b.Property(x => x.ViewMode)
            .HasColumnType("nvarchar(20)")
            .HasConversion(
                v => v == ViewMode.List ? "List" : "Grid",
                s => string.Equals(s, "List", System.StringComparison.OrdinalIgnoreCase) ? ViewMode.List : ViewMode.Grid
            )
            .HasDefaultValue(ViewMode.Grid)
            .IsRequired();

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();
    }
}
