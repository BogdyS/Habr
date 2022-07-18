using Habr.Common.DTO.User;
using Habr.DataAccess;
using Habr.WebAPI.BackgroundJobs.Context;
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

    public static void CreateBirthdayEmailJob(IRecurringJobManager jobManager, IEmailSender sender, IConfiguration configuration, UserDTO user)
    {
        var context = new EmailContext()
        {
            Name = user.Name,
            EmailTo = user.Email,
            SendingTime = user.DateOfBirth,
            Password = configuration["Email:Password"],
            EmailFrom = configuration["Email:EmailAddress"]
        };
        var day = user.DateOfBirth.Day;
        var month = user.DateOfBirth.Month;

        string cronExpression = $"0 12 {day} {month} *"; //cron expression for repeating every year in current birthday

       jobManager.AddOrUpdate($"birthdayEmail{user.Id}", () => sender.SendEmailAsync(context), cronExpression);
    }
}