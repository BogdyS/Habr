using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Mapping;
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

    public static IServiceCollection AddAutoMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(PostProfile).Assembly);
        services.AddAutoMapper(typeof(CommentProfile).Assembly);
        return services;
    }
}