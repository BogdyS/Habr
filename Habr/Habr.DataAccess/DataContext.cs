using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Habr.DataAccess.Configurations;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        public static readonly ILoggerFactory ConsoleLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public  DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(ConsoleLoggerFactory);
            try
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["HabrDatabase"].ConnectionString);
            }
            catch (NullReferenceException ex)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings[0].ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
        }
    }
}
