using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.Common.DTO.User;
using Habr.DataAccess;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebAPI.BackgroundJobs;

public static class CreateBackgroundJobs
{
    public static void CreatePostRatingDailyJob(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var ratingCalculator = scope.ServiceProvider.GetRequiredService<IPostRatingCalculator>();
        recurringJobManager.AddOrUpdate("ratingCalculation",
            () => ratingCalculator!.CalculateAverageRating(),
            Cron.Daily(6, 0), TimeZoneInfo.Utc);
    }

    public static void CreateBirthdayEmailJob(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        recurringJobManager.AddOrUpdate("birthdayEmail", () => emailSender.SendEmailAsync(),
            Cron.Daily(12, 0), TimeZoneInfo.Utc);
    }
}