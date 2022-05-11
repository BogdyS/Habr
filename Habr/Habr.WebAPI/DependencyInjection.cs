using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Servises;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        return services;
    }

    public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("HabrDatabase")),
            ServiceLifetime.Scoped);
        return services;
    }
}