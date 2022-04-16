using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.Name)
            .HasMaxLength(60)
            .IsRequired();
        builder.Property(x => x.Email)
            .HasMaxLength(264)
            .IsRequired();
    }
}