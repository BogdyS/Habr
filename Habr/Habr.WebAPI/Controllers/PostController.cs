﻿using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Route("api/v{version:apiVersion}post")]
    public class PostController : HabrController
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpPost("posts")]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatingPostDTO post)
        {
            CheckClaimId(HttpContext.User.Claims, post.UserId);

            var newPost = await _postService.CreatePostAsync(post);

            if (!post.IsDraft)
            {
                _logger.LogInformation($"Post published with userId = {post.UserId} ; postId = {newPost.Id}");
            }
            
            return CreatedAtAction(nameof(GetPostAsync), new { postId = newPost.Id }, newPost);
        }

        [AllowAnonymous]
        [HttpGet("posts/{postId:int}")]
        public async Task<IActionResult> GetPostAsync([FromRoute] int postId)
        {
            var post = await _postService.GetPostWithCommentsAsync(postId);
            return Ok(post);
        }

        [HttpGet("users/{userId:int}/posts")]
        public async Task<IActionResult> GetUserPostsAsync([FromRoute] int userId)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserPostsAsync(userId));
        }

        [HttpGet("users/{userId:int}/posts/drafts")]
        public async Task<IActionResult> GetUserDraftsAsync([FromRoute] int userId)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserDraftsAsync(userId));
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/public-from-drafts")]
        public async Task<IActionResult> PublicPostFromDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            await _postService.PostFromDraftAsync(postId, userId);
            _logger.LogInformation($"Post published with userId = {userId} ; postId = {postId}");
            return Ok();
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/remove-to-drafts")]
        public async Task<IActionResult> RemovePostToDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            await _postService.RemovePostToDraftsAsync(postId, userId);
            return Ok();
        }

        [HttpDelete("users/{userId:int}/posts/{postId:int}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            var role = GetUserRole(HttpContext.User.Claims, userId);

            await _postService.DeletePostAsync(postId, userId, role);
            return Ok();
        }

        [HttpPut("users/{userId:int}/posts/{postId:int}")]
        public async Task<IActionResult> UpdatePostAsync([FromRoute] int userId, [FromRoute] int postId, [FromBody] UpdatePostDTO post)
        {
            var role = GetUserRole(HttpContext.User.Claims, userId);

            await _postService.UpdatePostAsync(post, userId, postId, role);
            return Ok();
        }
    }
}
