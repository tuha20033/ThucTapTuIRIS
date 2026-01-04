using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class CategorySequenceConfiguration : IEntityTypeConfiguration<CategorySequence>
{
    public void Configure(EntityTypeBuilder<CategorySequence> b)
    {
        b.ToTable("CategorySequence");

        b.HasKey(x => new { x.UserId, x.CategoryId })
            .HasName("PK_UserCategoryOrders");

        b.Property(x => x.UserId)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        b.Property(x => x.OrderIndex)
            .IsRequired();
        b.Property(x => x.IsCollapsed)
            .HasDefaultValue(false)
        .IsRequired();

        b.HasOne(x => x.User)
            .WithMany(u => u.CategorySequences)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_CategorySequence_Users")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_CategorySequence_Categories")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.UserId, x.OrderIndex })
            .HasDatabaseName("IX_CategorySequence_UserId");
    }
}
