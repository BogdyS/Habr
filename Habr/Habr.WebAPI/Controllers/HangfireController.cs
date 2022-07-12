using Habr.WebAPI.BackgroundJobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/v{version:apiVersion}/background-jobs")]
[Tags("Background")]
public class HangfireController : ControllerBase
{
    private readonly IPostRatingCalculator _ratingCalculator;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    
    public HangfireController(IPostRatingCalculator ratingCalculator, IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager)
    {
        _ratingCalculator = ratingCalculator;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    [HttpGet("posts/rating-calculation")]
    public IActionResult RatingCalculation()
    {
        _recurringJobManager.AddOrUpdate("ratingCalculation", () => _ratingCalculator.CalculateAverageRating(), 
            Cron.Daily(6,0), TimeZoneInfo.Utc);

        return Ok();
    }
}