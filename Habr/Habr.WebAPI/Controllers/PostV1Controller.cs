using System.ComponentModel;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [ApiVersion("2")]
    [ApiVersion("3")]
    [Route("api/v{version:apiVersion}/post")]
    [Route("api/post")]
    [Tags("Post")]
    public class PostV1Controller : HabrController
    {
        protected readonly IPostService _postService;
        protected readonly ILogger<PostV1Controller> _logger;

        public PostV1Controller(IPostService postService, ILogger<PostV1Controller> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [MapToApiVersion("1")]
        [AllowAnonymous]
        [HttpGet("posts")]
        public virtual async Task<IActionResult> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsV1Async());
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

        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [HttpGet("users/{userId:int}/posts")]
        public virtual async Task<IActionResult> GetUserPostsAsync([FromRoute] int userId)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserPostsAsync(userId));
        }

        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        [HttpGet("users/{userId:int}/posts/drafts")]
        public virtual async Task<IActionResult> GetUserDraftsAsync([FromRoute] int userId)
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
