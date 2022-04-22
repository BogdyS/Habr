using Habr.DataAccess.Entities;
using Habr.DataAccess.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Servises
{
    public class PostService
    {
        public async Task<List<Post>> GetAllPosts()
        {
            using (var context = new DataContext())
            {
                return await context.Posts
                    .Include(p => p.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
        public async Task<List<Post>> GetUserPosts(int userId)
        {
            using (var context = new DataContext()) 
            {
                return await context.Posts
                    .Where(p => p.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
        public async Task<Post> GetPostWithComments(int id)
        {
            using (var context = new DataContext())
            {
                Post post;
                try
                {
                    post = await context.Posts
                        .Where(p => p.Id == id)
                        .Include(p => p.Comments)
                        .SingleAsync();
                    for (int i = 0; i < post.Comments.Count(); i++)
                    {
                        Comment comment = post.Comments.ElementAt(i);
                        if (comment.ParentCommentId == null)
                        {
                            await GetComments(comment.Id, context);
                        }
                    }
                    return post;
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
            }
        }
        private async Task GetComments(int commentId, DataContext context)
        {
            Comment comment = await context.Comments
                .Include(x => x.ParentComment)
                .Include(x => x.Comments)
                .Include(x => x.User)
                .SingleAsync(x => x.Id == commentId);
            for (int i = 0; i < comment.Comments.Count(); i++)
            {
                await GetComments(comment.Comments.ElementAt(i).Id, context);
            }
        }
        public async Task CreatePost(string title, string text, int userId)
        {
            using (var context = new DataContext())
            {
                await context.Posts.AddAsync(new Post()
                {
                    Text = text,
                    Title = title,
                    UserId = userId,
                    Created = DateTime.Now
                });
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdatePost(string newTitle, string newText, int postId, int userId)
        {
            using (var context = new DataContext())
            {
                Post postToUpdate;
                try
                {
                    postToUpdate = await context.Posts.SingleAsync(p => p.Id == postId);
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
                if (postToUpdate.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }
                postToUpdate.Text = newText;
                postToUpdate.Title = newTitle;
                await context.SaveChangesAsync();
            }
        }
        public async Task DeletePost(int postId, int userId)
        {
            using (var context = new DataContext())
            {
                Post post;
                try
                {
                    post = await context.Posts.SingleAsync(p => p.Id == postId);
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
                if (post.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }
                context.Posts.Remove(post);
                await context.SaveChangesAsync();
            }
        }
    }
}