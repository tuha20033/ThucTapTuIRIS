using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");

        b.HasKey(x => x.Id).HasName("PK_Categories");

        b.Property(x => x.Id)
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        b.Property(x => x.Name)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        b.HasAlternateKey(x => x.Name)
            .HasName("UQ_Categories_Name");

        b.Property(x => x.Description).HasColumnType("nvarchar(255)");

        b.Property(x => x.Priority)
            .HasDefaultValue(0)
            .IsRequired();

        b.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        b.HasMany(x => x.Links)
            .WithOne(x => x.Category!)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_Links_Categories")
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
