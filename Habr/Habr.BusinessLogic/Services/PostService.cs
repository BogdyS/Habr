﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using InvalidDataException = Habr.Common.Exceptions.InvalidDataException;

namespace Habr.BusinessLogic.Servises
{
    public class PostService : IPostService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IValidator<IPost> _postValidator;
        public PostService(DataContext dbContext, IMapper mapper, IUserService userService, IValidator<IPost> postValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userService = userService;
            _postValidator = postValidator;
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
                throw new NotFoundException($"User with id = {userId} doesn't exists");
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
                throw new NotFoundException($"User with id = {userId} doesn't exists");
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
                throw new NotFoundException("The post doesn't exists");
            }

            var post = _mapper.Map<FullPostDTO>(postEntity);

            return post;
        }

        public async Task<FullPostDTO> CreatePostAsync(CreatingPostDTO post)
        {
            var validationResult = await _postValidator.ValidateAsync(post);

            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                throw new InvalidDataException(error.ErrorMessage, (string) error.AttemptedValue);
            }

            User? user;

            if ((user = await _userService.IsUserExistsAsync(post.UserId)) is null)
            {
                throw new NotFoundException("User isn't exists");
            }

            var entity = _mapper.Map<Post>(post);
            entity.User = user;

            var dateTime = DateTime.UtcNow;
            entity.Created = dateTime;
            entity.Posted = dateTime;
            entity.Updated = dateTime;

            _dbContext.Posts.Add(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<FullPostDTO>(entity);
        }

        public async Task PostFromDraftAsync(int draftId, int userId)
        {
            Post? draft = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == draftId);

            if (draft == null)
            {
                throw new NotFoundException("The post doesn't exists");
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
                throw new NotFoundException("The post doesn't exists");
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

        public async Task UpdatePostAsync(UpdatePostDTO post)
        {
            var validationResult = await _postValidator.ValidateAsync(post);

            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                throw new InvalidDataException(error.ErrorMessage, (string)error.AttemptedValue);
            }

            Post? postToUpdate = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == post.PostId);

            if (postToUpdate == null)
            {
                throw new NotFoundException("The post doesn't exists");
            }

            if (postToUpdate.UserId != post.UserId)
            {
                throw new AccessException("User can't update another user's post");
            }

            if (!postToUpdate.IsDraft)
            {
                throw new AccessException("User can update only drafts");
            }

            postToUpdate.Text = post.Text!;
            postToUpdate.Title = post.Title!;
            postToUpdate.Updated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            Post? post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                throw new NotFoundException("The post doesn't exists");
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