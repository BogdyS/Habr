using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habr.DataAccess.Configurations
{
    class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(x => x.Created)
                .IsRequired();
            builder.Property(x => x.Updated)
                .IsRequired();
            builder.Property(x => x.Posted)
                .IsRequired();
            builder.Property(x => x.IsDraft)
                .IsRequired();
            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasMany(x => x.Rates)
                .WithOne(r => r.Post)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
