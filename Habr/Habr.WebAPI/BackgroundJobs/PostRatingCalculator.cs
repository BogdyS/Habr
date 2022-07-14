using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebAPI.BackgroundJobs;

public class PostRatingCalculator : IPostRatingCalculator
{
    private readonly DataContext _dataContext;

    public PostRatingCalculator(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task CalculateAverageRating()
    {
        await _dataContext.Posts
            .Include(p=>p.Rates)
            .ForEachAsync(CalculatePost);

        await _dataContext.SaveChangesAsync();
    }

    private void CalculatePost(Post post)
    {
        int total = post.Rates.Sum(x => x.Value);
        int count = post.Rates.Count();
        if (count == 0) count = 1;
        post.AverageRating = Math.Round((double)total/count, 2);
    }
}