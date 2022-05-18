﻿using Habr.Common.DTO;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostListDTO>?> GetAllPostsAsync();
    Task<IEnumerable<PostListDTO>?> GetUserPostsAsync(int userId);
    Task<IEnumerable<PostDraftDTO>?> GetUserDraftsAsync(int userId);
    Task<FullPostDTO> GetPostWithCommentsAsync(int postId);
    Task CreatePostAsync(string? title, string? text, bool isDraft, int userId);
    Task PostFromDraftAsync(int draftId, int userId);
    Task RemovePostToDraftsAsync(int postId, int userId);
    Task UpdatePostAsync(string newTitle, string newText, int postId, int userId);
    Task DeletePostAsync(int postId, int userId);
}