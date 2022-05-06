using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IPostService
{
    Task<List<PostListDTO>> GetAllPostsAsync();
    Task<List<PostListDTO>> GetUserPostsAsync(int userId);
    Task<List<PostDraftDTO>> GetUserDraftsAsync(int userId);
    Task<Post> GetPostWithCommentsAsync(int postId, ICommentService service);
    Task CreatePostAsync(string? title, string? text, bool isDraft, int userId);
    Task PostFromDraftAsync(int draftId, int userId);
    Task RemovePostToDraftsAsync(int postId, int userId);
    Task UpdatePostAsync(string newTitle, string newText, int postId, int userId);
    Task DeletePostAsync(int postId, int userId);
}