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
            .HasMaxLength(200)
            .IsRequired();
        builder.HasIndex(x => x.Email)
            .IsUnique();
        builder.Property(x => x.Password)
            .IsRequired();
        builder.Property(x => x.RefreshToken)
            .IsRequired(false);
        builder.Property(x => x.RefreshTokenActiveTo)
            .IsRequired(false);
        builder.Property(x => x.Role)
            .IsRequired()
            .HasDefaultValue(RolesEnum.User);
    }
}