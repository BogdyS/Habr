using Habr.DataAccess;
using Hangfire;

namespace Habr.WebAPI.BackgroundJobs;

public static class CreateBackgroundJobs
{
    public static void CreatePostRatingDailyJob(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetService<IRecurringJobManager>();
        var ratingCalculator = scope.ServiceProvider.GetService<IPostRatingCalculator>();
        recurringJobManager.AddOrUpdate("ratingCalculation",
            () => ratingCalculator!.CalculateAverageRating(),
            Cron.Daily(6, 0), TimeZoneInfo.Utc);
    }
}