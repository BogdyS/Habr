using AutoMapper;
using FluentValidation;
using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.Common.Resourses;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using InvalidDataException = Habr.Common.Exceptions.InvalidDataException;

namespace Habr.BusinessLogic.Servises;

public abstract class BasePostService
{
    protected readonly DataContext _dbContext;
    protected readonly IMapper _mapper;
    protected readonly IUserService _userService;
    protected readonly IValidator<IPostDTO> _postValidator;

    protected BasePostService(DataContext dbContext, IMapper mapper, IUserService userService,
        IValidator<IPostDTO> postValidator)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userService = userService;
        _postValidator = postValidator;
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
            throw new NotFoundException(ExceptionMessages.PostNotFound);
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
            throw new InvalidDataException(error.ErrorMessage, (string)error.AttemptedValue);
        }

        User? user;

        if ((user = await _userService.IsUserExistsAsync(post.UserId)) is null)
        {
            throw new NotFoundException(ExceptionMessages.UserNotFound);
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
            throw new NotFoundException(ExceptionMessages.PostNotFound);
        }

        if (draft.UserId != userId)
        {
            throw new BusinessLogicException(ExceptionMessages.AcessToPostDenied);
        }

        if (!draft.IsDraft)
        {
            throw new BusinessLogicException(ExceptionMessages.PostNotDraft);
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
            throw new NotFoundException(ExceptionMessages.PostNotFound);
        }

        if (post.UserId != userId)
        {
            throw new BusinessLogicException(ExceptionMessages.AcessToPostDenied);
        }

        if (post.IsDraft)
        {
            throw new BusinessLogicException(ExceptionMessages.PostAlreadyDraft);
        }

        bool hasComments = await _dbContext.Comments.AnyAsync(x => x.PostId == postId);
        if (hasComments)
        {
            throw new BusinessLogicException(ExceptionMessages.RemovingPostWithComments);
        }

        post.Updated = DateTime.UtcNow;
        post.IsDraft = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(UpdatePostDTO post, int userId, int postId, RolesEnum role)
    {
        var validationResult = await _postValidator.ValidateAsync(post);

        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            throw new InvalidDataException(error.ErrorMessage, (string)error.AttemptedValue);
        }

        Post? postToUpdate = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId);

        if (postToUpdate == null)
        {
            throw new NotFoundException(ExceptionMessages.PostNotFound);
        }

        if (postToUpdate.UserId != userId && role != RolesEnum.Admin)
        {
            throw new BusinessLogicException(ExceptionMessages.AcessToPostDenied);
        }

        if (!postToUpdate.IsDraft)
        {
            throw new BusinessLogicException(ExceptionMessages.UpdatingNotDraftPost);
        }

        postToUpdate.Text = post.Text!;
        postToUpdate.Title = post.Title!;
        postToUpdate.Updated = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePostAsync(int postId, int userId, RolesEnum role)
    {
        Post? post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new NotFoundException(ExceptionMessages.PostNotFound);
        }

        if (post.UserId != userId && role != RolesEnum.Admin)
        {
            throw new BusinessLogicException(ExceptionMessages.AcessToPostDenied);
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }
}
