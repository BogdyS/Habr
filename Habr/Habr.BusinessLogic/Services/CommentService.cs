using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class CommentService
    {
        public async Task<List<Comment>> GetComments(int postId)
        {
            using (var context = new DataContext())
            {
                var comments = await context.Comments
                    .Where(c => c.PostId == postId)
                    .Include(x => x.User)
                    .ToListAsync();

                foreach (var comment in comments)
                {
                    await GetComments(comment.Id, context);
                }

                return comments;
            }
        }
        public async Task CreateCommentToPost(int postId, int userId, string text)
        {
            using (var context = new DataContext())
            {
                if (!await context.Posts.AnyAsync(x => x.Id == postId))
                {
                    throw new SQLException("Post not found");
                }

                await context.Comments.AddAsync(new Comment()
                {
                    PostId = postId,
                    UserId = userId,
                    Text = text,
                    Created = DateTime.Now,
                    ParentCommentId = null
                });
                await context.SaveChangesAsync();
            }
        }
        public async Task CreateCommentToComment(int postId, int commentId, int userId, string text)
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

                await context.Comments.AddAsync(new Comment()
                {
                    PostId = postId,
                    UserId = userId,
                    Text = text,
                    Created = DateTime.Now,
                    ParentCommentId = commentId
                });
                await context.SaveChangesAsync();
            }
        }
        public async Task DeleteComment(int commentId, int userId)
        {
            using (var context = new DataContext())
            {
                if (!await IsCommentExistsAsync(commentId, context))
                {
                    throw new SQLException("Comment not found");
                }

                Comment comment = await context.Comments.SingleAsync(c => c.Id == commentId);
                ;

                if (comment.UserId != userId)
                {
                    throw new AccessException("User can't delete another user's comment");
                }

                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
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