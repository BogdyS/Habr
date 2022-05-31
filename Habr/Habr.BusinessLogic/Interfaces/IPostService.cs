﻿using Habr.Common.DTO;

namespace Habr.BusinessLogic.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostListDTO>?> GetAllPostsAsync();
    Task<IEnumerable<PostListDTO>?> GetUserPostsAsync(int userId);
    Task<IEnumerable<PostDraftDTO>?> GetUserDraftsAsync(int userId);
    Task<FullPostDTO> GetPostWithCommentsAsync(int postId);
    Task<FullPostDTO> CreatePostAsync(CreatingPostDTO post);
    Task PostFromDraftAsync(int draftId, int userId);
    Task RemovePostToDraftsAsync(int postId, int userId);
    Task UpdatePostAsync(UpdatePostDTO post);
    Task DeletePostAsync(int postId, int userId);
}