using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class CommentService : ICommentService
    {
        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId)
        {
            using (var context = new DataContext())
            {
                var commentsEntity = await context.Comments
                    .Where(c => c.PostId == postId)
                    .Where(c => c.ParentCommentId == null)
                    .Include(c => c.User)
                    .ToListAsync();

                var comments = new List<CommentDTO>();

                foreach (var c in commentsEntity)
                {
                    var commentDto = new CommentDTO
                    {
                        Text = c.Text,
                        AuthorName = c.User.Name,
                        Comments = await GetCommentsAsync(c.Id, context)
                    };
                    comments.Add(commentDto);
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

        private async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int commentId, DataContext context)
        {
            var commentChildEntity = await context.Comments
                .Include(x => x.Comments)
                .Where(x => x.ParentCommentId == commentId)
                .Include(x => x.User)
                .AsNoTracking()
                .ToListAsync();

            if (commentChildEntity.Count == 0)
            {
                return new List<CommentDTO>();
            }

            var childComments = new List<CommentDTO>();

            foreach (var c in commentChildEntity)
            {
                var commentDto = new CommentDTO()
                {
                    Text = c.Text,
                    AuthorName = c.User.Name
                };
                commentDto.Comments = await GetCommentsAsync(c.Id, context);
                childComments.Add(commentDto);
            }

            return childComments;
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