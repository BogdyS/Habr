using Habr.Common.DTO;

namespace Habr.BusinessLogic.Interfaces;

public interface ICommentService
{
    Task<CommentDTO> GetCommentAsync(int id);
    Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDto);
    Task DeleteCommentAsync(int commentId, int userId);
}