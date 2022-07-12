using Habr.Common.DTO;
using Habr.Common.DTO.Pagination;

namespace Habr.BusinessLogic.Interfaces;

public interface IPagedPostService
{
    Task<PaginatedData<PostListDtoV2>> GetAllPostsPageAsync(int pageNumber, int pageSize);
    Task<PaginatedData<PostListDtoV1>?> GetUserPostsPageAsync(int userId, int pageNumber, int pageSize);
    Task<PaginatedData<PostDraftDTO>?> GetUserDraftsPageAsync(int userId, int pageNumber, int pageSize);
}