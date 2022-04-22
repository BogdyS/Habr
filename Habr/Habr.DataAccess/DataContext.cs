using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Habr.DataAccess.Configurations;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        //public static readonly ILoggerFactory ConsoleLoggerFactory =
        //    LoggerFactory.Create(builder =>
        //    {
        //        builder.AddConsole();
        //        builder.SetMinimumLevel(LogLevel.Information);
        //    });

        private static readonly IConfiguration configuration =
            new ConfigurationBuilder().AddJsonFile(@"appsettings.json").Build();
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public  DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(ConsoleLoggerFactory);
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("HabrDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostConfiguration).Assembly);
        }
    }
}
