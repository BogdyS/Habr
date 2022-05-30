using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public PostService(DataContext dbContext, IMapper mapper, IUserService userService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<PostListDTO>?> GetAllPostsAsync()
        {
            var posts = await _dbContext.Posts
                .Where(p => !p.IsDraft)
                .Include(p => p.User)
                .ProjectTo<PostListDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>?> GetUserPostsAsync(int userId)
        {
            if (await _userService.IsUserExistsAsync(userId) is null)
            {
                throw new SQLException($"User with id = {userId} doesn't exists");
            }

            var posts = await _dbContext.Posts
                .Where(p => p.UserId == userId)
                .Where(p => !p.IsDraft)
                .ProjectTo<PostListDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostDraftDTO>?> GetUserDraftsAsync(int userId)
        {
            if (await _userService.IsUserExistsAsync(userId) is null)
            {
                throw new SQLException($"User with id = {userId} doesn't exists");
            }

            var posts = await _dbContext.Posts
                .Where(p => p.UserId == userId)
                .Where(p => p.IsDraft)
                .ProjectTo<PostDraftDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return posts;
        }

        public async Task<FullPostDTO> GetPostWithCommentsAsync(int postId)
        {
            var postEntity = await _dbContext.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.User)
                .Include(p => p.Comments.Where(c => c.PostId == postId))
                .ThenInclude(comment => comment.User)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (postEntity == null)
            {
                throw new SQLException("The post doesn't exists");
            }

            var post = _mapper.Map<FullPostDTO>(postEntity);

            return post;
        }

        public async Task<FullPostDTO> CreatePostAsync(CreatingPostDTO post)
        {
            if (post.Title == null)
            {
                throw new InputException("The Title is required");
            }

            if (post.Text == null)
            {
                throw new InputException("The Text is required");
            }

            if (!PostValidation.TitleValidation(post.Title))
            {
                throw new InputException($"The Title must be less than {PostValidation.MaxTitleLength} symbols");
            }

            if (!PostValidation.TextValidation(post.Text))
            {
                throw new InputException($"The Text must be less than {PostValidation.MaxTextLength} symbols");
            }

            User? user;

            if ((user = await _userService.IsUserExistsAsync(post.UserId)) is null)
            {
                throw new InputException("User isn't exists");
            }

            var entity = _mapper.Map<Post>(post);
            entity.User = user;

            _dbContext.Posts.Add(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<FullPostDTO>(entity);
        }

        public async Task PostFromDraftAsync(int draftId, int userId)
        {
            Post? draft = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == draftId);

            if (draft == null)
            {
                throw new SQLException("The post doesn't exists");
            }

            if (draft.UserId != userId)
            {
                throw new AccessException("User can't update another user's post");
            }

            if (!draft.IsDraft)
            {
                throw new AccessException("Post isn't draft");
            }

            var time = DateTime.UtcNow;

            draft.IsDraft = false;
            draft.Updated = time;
            draft.Posted = time;

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemovePostToDraftsAsync(int postId, int userId)
        {
            Post? post = await _dbContext.Posts
                .SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                throw new SQLException("The post doesn't exists");
            }

            if (post.UserId != userId)
            {
                throw new AccessException("User can't update another user's post");
            }

            if (post.IsDraft)
            {
                throw new AccessException("Post is already draft");
            }

            bool hasComments = await _dbContext.Comments.AnyAsync(x => x.PostId == postId);
            if (hasComments)
            {
                throw new AccessException("User can't remove post with comments to drafts");
            }

            post.Updated = DateTime.UtcNow;
            post.IsDraft = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(string? newTitle, string? newText, int postId, int userId)
        {
            Post? postToUpdate = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId);

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
                throw new InputException($"The Title must be less than {PostValidation.MaxTitleLength} symbols and not empty");
            }

            if (!PostValidation.TextValidation(newText))
            {
                throw new InputException($"The Text must be less than {PostValidation.MaxTextLength} symbols and not empty");
            }

            postToUpdate.Text = newText;
            postToUpdate.Title = newTitle;
            postToUpdate.Updated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            Post? post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                throw new SQLException("The post doesn't exists");
            }

            if (post.UserId != userId)
            {
                throw new AccessException("User can't update another user's post");
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }
    }
}