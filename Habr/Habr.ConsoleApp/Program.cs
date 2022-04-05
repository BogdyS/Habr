using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Habr.ConsoleApp
{
    class Program
    {
        async static Task Main(string[] args)
        {
            using (var context = new DataContext())
            {
                await context.Database.MigrateAsync();
            }
        }
    }
}

