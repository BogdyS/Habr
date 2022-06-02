using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Habr.DataAccess.Configurations;
using Microsoft.Extensions.Configuration;

namespace Habr.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostConfiguration).Assembly);
        }
    }
}
