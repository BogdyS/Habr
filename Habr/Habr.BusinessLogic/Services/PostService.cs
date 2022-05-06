using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Validation;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class PostService : IPostService
    {
        public async Task<List<PostListDTO>> GetAllPostsAsync()
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => !p.IsDraft)
                    .Include(p => p.User)
                    .Select(post => new PostListDTO()
                    {
                        Posted = post.Posted,
                        Title = post.Title,
                        UserEmail = post.User.Email
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return posts;
            }
        }

        public async Task<List<PostListDTO>> GetUserPostsAsync(int userId)
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => p.UserId == userId)
                    .Where(p => !p.IsDraft)
                    .Select(post => new PostListDTO()
                    {
                        Posted = post.Posted,
                        Title = post.Title,
                        UserEmail = post.User.Email
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return posts;
            }
        }

        public async Task<List<PostDraftDTO>> GetUserDraftsAsync(int userId)
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => p.UserId == userId)
                    .Where(p => p.IsDraft)
                    .Select(post => new PostDraftDTO()
                    {
                        Created = post.Created,
                        Title = post.Title,
                        Updated = post.Updated
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return posts;
            }
        }

        public async Task<FullPostDTO> GetPostWithCommentsAsync(int postId, ICommentService commentService)
        {
            using (var context = new DataContext())
            {
                var postEntity = await context.Posts
                    .Where(p => p.Id == postId)
                    .Include(p=>p.User)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

                if (postEntity == null)
                {
                    throw new SQLException("The post doesn't exists");
                }

                var post = new FullPostDTO
                {
                    Text = postEntity.Text,
                    Title = postEntity.Title,
                    AuthorEmail = postEntity.User.Email,
                    PublishDate = postEntity.Posted,
                    Comments = await commentService.GetCommentsAsync(postId)
                };

                return post;
            }
        }

        public async Task CreatePostAsync(string? title, string? text, bool isDraft, int userId)
        {
            using (var context = new DataContext())
            {
                if (title == null)
                {
                    throw new InputException("The Title is required");
                }

                if (text == null)
                {
                    throw new InputException("The Text is required");
                }

                if (!PostValidation.TitleValidation(title))
                {
                    throw new InputException($"The Title must be less than {PostValidation.MaxTitleLength} symbols");
                }

                if (!PostValidation.TextValidation(text))
                {
                    throw new InputException($"The Text must be less than {PostValidation.MaxTextLength} symbols");
                }
                var dateTime = DateTime.UtcNow;
                context.Posts.Add(new Post()
                {
                    Text = text,
                    Title = title,
                    UserId = userId,
                    Created = dateTime,
                    Posted =  dateTime,
                    Updated = dateTime,
                    IsDraft = isDraft
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task PostFromDraftAsync(int draftId, int userId)
        {
            using (var context = new DataContext())
            {
                Post? draft = await context.Posts.SingleOrDefaultAsync(p => p.Id == draftId);

                if (draft == null)
                {
                    throw new SQLException("The post doesn't exists");
                }

                if (draft.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }

                if (draft.IsDraft)
                {
                    throw new AccessException("Post is already draft");
                }

                var time = DateTime.UtcNow;

                draft.IsDraft = false;
                draft.Updated = time;
                draft.Posted = time;
                await context.SaveChangesAsync();
            }
        }

        public async Task RemovePostToDraftsAsync(int postId, int userId)
        {
            using (var context = new DataContext())
            {
                Post? post = await context.Posts
                    .SingleOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    throw new SQLException("The post doesn't exists");
                }

                if (post.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }

                if (!post.IsDraft)
                {
                    throw new AccessException("Post isn't draft already");
                }

                bool hasComments = await context.Comments.AnyAsync(x => x.PostId == postId);
                if (hasComments)
                {
                    throw new AccessException("User can't remove post with comments to drafts");
                }

                post.Updated = DateTime.UtcNow;
                post.IsDraft = true;
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdatePostAsync(string newTitle, string newText, int postId, int userId)
        {
            using (var context = new DataContext())
            {
                Post? postToUpdate = await context.Posts.SingleOrDefaultAsync(p => p.Id == postId);

                if (postToUpdate == null)
                {
                    throw new SQLException("The post doesn't exists");
                }

                if (postToUpdate.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }

                if (!postToUpdate.IsDraft)
                {
                    throw new AccessException("User can update only drafts");
                }

                if (!PostValidation.TitleValidation(newTitle))
                {
                    throw new InputException($"The Title must be less than {PostValidation.MaxTitleLength} symbols");
                }

                if (!PostValidation.TextValidation(newText))
                {
                    throw new InputException($"The Text must be less than {PostValidation.MaxTextLength} symbols");
                }

                postToUpdate.Text = newText;
                postToUpdate.Title = newTitle;
                postToUpdate.Updated = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            using (var context = new DataContext())
            {
                Post? post = await context.Posts.SingleOrDefaultAsync(p => p.Id == postId);
                
                if (post == null)
                {
                    throw new SQLException("The post doesn't exists");
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