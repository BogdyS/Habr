using Habr.Common;
using Habr.Common.DTO;

namespace Habr.BusinessLogic.Interfaces;

public interface ICommentService
{
    Task<CommentDTO> GetCommentAsync(int id);
    Task<CommentDTO> CreateCommentAsync(CreateCommentDTO commentDto);
    Task UpdateCommentAsync(string commentText, int commentId, int userId, RolesEnum role);
    Task DeleteCommentAsync(int commentId, int userId, RolesEnum role);
}