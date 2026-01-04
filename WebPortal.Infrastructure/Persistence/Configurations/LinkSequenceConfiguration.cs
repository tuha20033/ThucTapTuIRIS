using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebPortal.Domain.Entities;

namespace WebPortal.Infrastructure.Persistence.Configurations;

public sealed class LinkSequenceConfiguration : IEntityTypeConfiguration<LinkSequence>
{
    public void Configure(EntityTypeBuilder<LinkSequence> b)
    {
        b.ToTable("LinkSequence");

        b.HasKey(x => new { x.UserId, x.LinkId })
            .HasName("PK_LinkSequence");

        b.Property(x => x.UserId)
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        b.Property(x => x.OrderIndex)
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany(u => u.LinkSequences)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_LinkSequence_Users")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Link)
            .WithMany(l => l.LinkSequences)
            .HasForeignKey(x => x.LinkId)
            .HasConstraintName("FK_LinkSequence_Links")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.UserId, x.OrderIndex })
            .HasDatabaseName("IX_LinkSequence_UserId");
    }
}
