using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class UserCategoryStateConfiguration : IEntityTypeConfiguration<UserCategoryState>
{
    public void Configure(EntityTypeBuilder<UserCategoryState> b)
    {
        b.ToTable("UserCategoryStates");

        b.HasKey(x => new { x.UserId, x.CategoryId });

        b.Property(x => x.UserId)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        b.Property(x => x.CategoryId)
            .IsRequired();

        b.Property(x => x.IsCollapsed)
            .HasDefaultValue(false)
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        b.HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
