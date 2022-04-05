using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.ConsoleApp
{
    class Program
    {
        async static Task Main(string[] args)
        {
            using (var context = new DataContext())
            {
                Post post = new Post()
                {
                    Title = "Первый Пост на Хабре",
                    Text = "Автор:Шапошников Богдан",
                    Created = DateTime.Now
                };
                await context.Posts.AddAsync(post);
                context.SaveChanges();
            }
        }
    }
}

