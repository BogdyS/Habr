using Habr.Common;
using Habr.Common.DTO;
using Habr.Common.DTO.Pagination;

namespace Habr.BusinessLogic.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostListDtoV1>?> GetAllPostsV1Async();
    Task<IEnumerable<PostListDtoV2>> GetAllPostsV2Async();
    Task<IEnumerable<PostListDtoV1>?> GetUserPostsAsync(int userId);
    Task<IEnumerable<PostDraftDTO>?> GetUserDraftsAsync(int userId);
    Task<PaginatedDTO<PostListDtoV2>> GetAllPostsPageAsync(int pageNumber, int pageSize);
    Task<PaginatedDTO<PostListDtoV1>?> GetUserPostsPageAsync(int userId, int pageNumber, int pageSize);
    Task<PaginatedDTO<PostDraftDTO>?> GetUserDraftsPageAsync(int userId, int pageNumber, int pageSize);
    Task<FullPostDTO> GetPostWithCommentsAsync(int postId);
    Task<FullPostDTO> CreatePostAsync(CreatingPostDTO post);
    Task PostFromDraftAsync(int draftId, int userId);
    Task RemovePostToDraftsAsync(int postId, int userId);
    Task UpdatePostAsync(UpdatePostDTO post, int userId, int postId, RolesEnum role);
    Task DeletePostAsync(int postId, int userId, RolesEnum role);
}