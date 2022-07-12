using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations;

public class RateConfiguration :IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(r => r.PostId)
            .IsRequired();
        builder.Property(r => r.UserId)
            .IsRequired();
        builder.Property(r => r.Value)
            .IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Rates)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Post)
            .WithMany(p => p.Rates)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}