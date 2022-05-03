using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface ICommentService
{
    Task<List<Comment>> GetCommentsAsync(int postId);
    Task CreateCommentToPostAsync(int postId, int userId, string text);
    Task CreateCommentToCommentAsync(int postId, int commentId, int userId, string text);
    Task DeleteCommentAsync(int commentId, int userId);
}