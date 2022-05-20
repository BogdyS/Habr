using Habr.Common.DTO;

namespace Habr.BusinessLogic.Interfaces;

public interface ICommentService
{
    Task<int> CreateCommentToPostAsync(CreateCommentDTO commentDto);
    Task<int> CreateCommentToCommentAsync(CreateCommentToCommentDTO commentDto);
    Task DeleteCommentAsync(int commentId, int userId);
}