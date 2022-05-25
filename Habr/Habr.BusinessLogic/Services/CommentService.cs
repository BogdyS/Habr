using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Habr.BusinessLogic.Servises
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;

        public CommentService(DataContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDto)
        {
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
                throw new SQLException("Comment not found");
            }

            if (comment.UserId != userId)
            {
                throw new AccessException("User can't delete another user's comment");
            }

            await CascadeDelete(comment);

            await _dbContext.SaveChangesAsync();
        }

        private async Task<CommentDTO> CreateCommentToPostAsync(CreateCommentDTO commentDto)
        {
            if (!await IsPostExistsAsync(commentDto.PostId))
            {
                throw new SQLException("Post not found");
            }

            User? user;
            if ((user = await IsUserExistsAsync(commentDto.UserId)) is null)
            {
                throw new SQLException("User not found");
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
                throw new SQLException("Post not found");
            }

            if (!await IsCommentAndPostValid(commentDto))
            {
                throw new SQLException("Invalid relationship between comment and post");
            }

            User? user;
            if ((user = await IsUserExistsAsync(commentDto.UserId)) is null)
            {
                throw new SQLException("User not found");
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

        private async Task<bool> IsCommentAndPostValid(CreateCommentDTO comment)
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