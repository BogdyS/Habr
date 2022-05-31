using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Habr.Common.Resourses;
using InvalidDataException = Habr.Common.Exceptions.InvalidDataException;

namespace Habr.BusinessLogic.Servises
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCommentDTO> _commentValidator;

        public CommentService(DataContext dbContext, IMapper mapper, IValidator<CreateCommentDTO> commentValidator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _commentValidator = commentValidator;
        }

        public async Task<CommentDTO> GetCommentAsync(int id)
        {
            var comment =  await _dbContext.Comments
                .ProjectTo<CommentDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(comment => comment.Id == id);

            if (comment == null)
            {
                throw new NotFoundException(ExceptionMessages.CommentNotFound);
            }
            
            return comment;
        }

        public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDto)
        {
            var validationResult = await _commentValidator.ValidateAsync(commentDto);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                throw new InvalidDataException(error.ErrorMessage, (string)error.AttemptedValue);
            }

            if (commentDto.ParentCommentId == default)
            {
                return await CreateCommentToPostAsync(commentDto);
            }

            return await CreateCommentToCommentAsync(commentDto);
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            Comment? comment = await _dbContext.Comments.SingleOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new NotFoundException(ExceptionMessages.CommentNotFound);
            }

            if (comment.UserId != userId)
            {
                throw new AccessException(ExceptionMessages.AcessToCommentDenied);
            }

            await CascadeDelete(comment);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<CommentDTO> CreateCommentToPostAsync(CreateCommentDTO commentDto)
        {
            if (!await IsPostExistsAsync(commentDto.PostId))
            {
                throw new NotFoundException(ExceptionMessages.PostNotFound);
            }

            User? user;
            if ((user = await IsUserExistsAsync(commentDto.UserId)) is null)
            {
                throw new NotFoundException(ExceptionMessages.UserNotFound);
            }

            var comment = _mapper.Map<Comment>(commentDto);
            comment.User = user;

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CommentDTO>(comment);
        }

        private async Task<CommentDTO> CreateCommentToCommentAsync(CreateCommentDTO commentDto)
        {
            if (!await IsPostExistsAsync(commentDto.PostId))
            {
                throw new NotFoundException(ExceptionMessages.PostNotFound);
            }

            if (!await IsCommentAndPostValidRelationship(commentDto))
            {
                throw new RelationshipException(ExceptionMessages.InvalidCommentPostRelationship);
            }

            User? user;
            if ((user = await IsUserExistsAsync(commentDto.UserId)) is null)
            {
                throw new NotFoundException(ExceptionMessages.UserNotFound);
            }

            var comment = _mapper.Map<Comment>(commentDto);
            comment.User = user;

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CommentDTO>(comment);
        }

        private async Task<bool> IsPostExistsAsync(int postId)
        {
            return await _dbContext.Posts.AnyAsync(x => x.Id == postId);
        }

        private async Task<User?> IsUserExistsAsync(int userId)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);
        }

        private async Task<bool> IsCommentAndPostValidRelationship(CreateCommentDTO comment)
        {
            int? parentCommentPostId = (await _dbContext.Comments
                .Select(c => new { c.PostId, c.Id })
                .SingleOrDefaultAsync(c => c.Id == comment.ParentCommentId))?.PostId;

            return parentCommentPostId == comment.PostId;
        }

        private async Task CascadeDelete(Comment comment)
        {
            var comments = await _dbContext.Comments
                .Where(c => c.ParentCommentId == comment.Id)
                .ToListAsync();

            foreach (var c in comments)
            {
                await CascadeDelete(c);
            }

            _dbContext.Remove(comment);
        }
    }
}