using Habr.BusinessLogic.Interfaces;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class CommentService : ICommentService
    {
        public async Task<List<Comment>> GetCommentsAsync(int postId)
        {
            using (var context = new DataContext())
            {
                var comments = await context.Comments
                    .Where(c => c.PostId == postId)
                    .Include(x => x.User)
                    .ToListAsync();

                foreach (var comment in comments)
                {
                    await GetCommentsAsync(comment.Id, context);
                }

                return comments;
            }
        }

        public async Task CreateCommentToPostAsync(int postId, int userId, string text)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("Post not found");
                }

                context.Comments.Add(new Comment()
                {
                    PostId = postId,
                    UserId = userId,
                    Text = text,
                    Created = DateTime.UtcNow,
                    ParentCommentId = null
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task CreateCommentToCommentAsync(int postId, int commentId, int userId, string text)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("Post not found");
                }

                if (!await IsCommentExistsAsync(commentId, context))
                {
                    throw new SQLException("Comment not found");
                }

                context.Comments.Add(new Comment()
                {
                    PostId = postId,
                    UserId = userId,
                    Text = text,
                    Created = DateTime.UtcNow,
                    ParentCommentId = commentId
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            using (var context = new DataContext())
            {
                Comment? comment = await context.Comments.SingleOrDefaultAsync(c => c.Id == commentId);

                if (comment == null)
                {
                    throw new SQLException("Comment not found");
                }

                if (comment.UserId != userId)
                {
                    throw new AccessException("User can't delete another user's comment");
                }

                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        private async Task GetCommentsAsync(int commentId, DataContext context)
        {
            Comment? comment = await context.Comments
                .Include(x => x.ParentComment)
                .Include(x => x.Comments)
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Id == commentId);

            if (comment == null)
            {
                throw new SQLException("Comment not found");
            }

            for (int i = 0; i < comment.Comments.Count(); i++)
            {
                await GetCommentsAsync(comment.Comments.ElementAt(i).Id, context);
            }
        }

        private async Task<bool> IsPostExistsAsync(int postId, DataContext context)
        {
            return await context.Posts.AnyAsync(x => x.Id == postId);
        }

        private async Task<bool> IsCommentExistsAsync(int commentId, DataContext context)
        {
            return await context.Comments.AnyAsync(x => x.Id == commentId);
        }
    }
}