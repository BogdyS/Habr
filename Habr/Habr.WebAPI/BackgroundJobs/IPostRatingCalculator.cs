namespace Habr.WebAPI.BackgroundJobs;

public interface IPostRatingCalculator
{
    Task CalculateAverageRating();
}