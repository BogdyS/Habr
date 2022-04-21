using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Servises;

public class CommentService
{
    public List<Comment> GetComments(int postId)
    {
        using (var context = new DataContext())
        {
            List<Comment> comments = context.Comments.Where(c => c.PostId == postId).ToList();
            List<int> userId = new List<int>();
            foreach (Comment comment in comments)
            {
                if (!userId.Contains(comment.UserId))
                {
                    userId.Add(comment.UserId);
                }
            }

            foreach (var id in userId)
            {
                context.Users.Where(u => u.Id == id).Load();
            }

            return comments;
        }
    }
    public async Task CreateComment(Comment comment)
    {
        using (var context = new DataContext())
        {
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteComment(int commentId)
    {
        using (var context = new DataContext())
        {
            Comment comment;
            try
            {
                comment = context.Comments.Single(c => c.Id == commentId);
            }
            catch (InvalidOperationException)
            {
                throw new SQLException("Comment not found");
            }
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}