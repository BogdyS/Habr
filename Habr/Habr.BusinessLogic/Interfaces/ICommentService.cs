using Habr.Common.DTO;

namespace Habr.BusinessLogic.Interfaces;

public interface ICommentService
{
    Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDto);
    Task DeleteCommentAsync(int commentId, int userId);
}