using System.Text;
using FluentValidation;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Mapping;
using Habr.BusinessLogic.Servises;
using Habr.BusinessLogic.Validation;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Habr.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IJwtService, JwtService>();
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
        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<IPostDTO>, PostValidator>();
        services.AddScoped<IValidator<RegistrationDTO>, UserValidator>();
        services.AddScoped<IValidator<CreateCommentDTO>, CommentValidator>();
        return services;
    }

    public static IServiceCollection AddFilters(this IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add<ExceptionFilter>());
        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication(config =>
            {
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ClockSkew = new TimeSpan(0,0,0,15)
                };
            });
        return services;
    }
}