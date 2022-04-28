using Habr.BusinessLogic.Validation;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class PostService
    {
        public async Task<List<PostListDTO>> GetAllPosts()
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => !p.IsDraft)
                    .Include(p => p.User)
                    .AsNoTracking()
                    .ToListAsync();

                var list = new List<PostListDTO>();
                foreach (var post in posts)
                {
                    list.Add(new PostListDTO()
                    {
                        Created = post.Created,
                        Title = post.Title,
                        UserEmail = post.User.Email
                    });
                }
                return list;
            }
        }
        public async Task<List<PostListDTO>> GetUserPosts(int userId)
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => p.UserId == userId)
                    .Where(p => !p.IsDraft)
                    .AsNoTracking()
                    .ToListAsync();

                var list = new List<PostListDTO>();
                foreach (var post in posts)
                {
                    list.Add(new PostListDTO()
                    {
                        Created = post.Created,
                        Title = post.Title,
                        UserEmail = post.User.Email
                    });
                }
                return list;
            }
        }

        public async Task<List<PostDraftDTO>> GetUserDrafts(int userId)
        {
            using (var context = new DataContext())
            {
                var posts = await context.Posts
                    .Where(p => p.UserId == userId)
                    .Where(p => p.IsDraft)
                    .AsNoTracking()
                    .ToListAsync();
                var list = new List<PostDraftDTO>();
                foreach (var post in posts)
                {
                    list.Add(new PostDraftDTO()
                    {
                        Created = post.Created,
                        Title = post.Title,
                        Updated = post.Updated ?? post.Created
                    });
                }
                return list;
            }
        }
        public async Task<Post> GetPostWithComments(int postId)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("The post doesn't exists");
                }

                Post post = await context.Posts
                    .Where(p => p.Id == postId)
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
        }
        public async Task CreatePost(string? title, string? text, bool isDraft, int userId)
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

                await context.Posts.AddAsync(new Post()
                {
                    Text = text,
                    Title = title,
                    UserId = userId,
                    Created = DateTime.Now,
                    IsDraft = isDraft
                });
                await context.SaveChangesAsync();
            }
        }
        public async Task PostDraft(int draftId, int userId)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(draftId, context))
                {
                    throw new SQLException("The post doesn't exists");
                }

                Post draft = await context.Posts.SingleAsync(p => p.Id == draftId);

                if (draft.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }

                if (draft.IsDraft)
                {
                    throw new AccessException("Post is already draft");
                }

                draft.IsDraft = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task RemovePostToDrafts(int postId, int userId)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("The post doesn't exists");
                }

                Post post = await context.Posts
                    .SingleAsync(p => p.Id == postId);

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

                post.IsDraft = true;
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdatePost(string newTitle, string newText, int postId, int userId)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("The post doesn't exists");
                }

                Post postToUpdate = await context.Posts.SingleAsync(p => p.Id == postId);

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
                await context.SaveChangesAsync();
            }
        }
        public async Task DeletePost(int postId, int userId)
        {
            using (var context = new DataContext())
            {
                if (!await IsPostExistsAsync(postId, context))
                {
                    throw new SQLException("The post doesn't exists");
                }

                Post post = await context.Posts.SingleAsync(p => p.Id == postId);

                if (post.UserId != userId)
                {
                    throw new AccessException("User can't update another user's post");
                }

                context.Posts.Remove(post);
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
    }
}