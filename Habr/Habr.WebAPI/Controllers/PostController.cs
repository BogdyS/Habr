using System.Security.Claims;
using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.Common.Resourses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/post-management")]
    public class PostController : ControllerBase
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
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, post.UserId))
            {
                return Forbid();
            }

            var newPost = await _postService.CreatePostAsync(post);

            _logger.LogInformation($"Post published with userId = {post.UserId} ; postId = {newPost.Id}");

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
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            return Ok(await _postService.GetUserPostsAsync(userId));
        }

        [HttpGet("users/{userId:int}/posts/drafts")]
        public async Task<IActionResult> GetUserDraftsAsync([FromRoute] int userId)
        {
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            return Ok(await _postService.GetUserDraftsAsync(userId));
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/public-from-drafts")]
        public async Task<IActionResult> PublicPostFromDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            await _postService.PostFromDraftAsync(postId, userId);
            return Ok();
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/remove-to-drafts")]
        public async Task<IActionResult> RemovePostToDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            await _postService.RemovePostToDraftsAsync(postId, userId);
            return Ok();
        }

        [HttpDelete("users/{userId:int}/posts/{postId:int}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            await _postService.DeletePostAsync(postId, userId);
            return Ok();
        }

        [HttpPut("users/{userId:int}/posts/{postId:int}")]
        public async Task<IActionResult> UpdatePostAsync([FromRoute] int userId, [FromRoute] int postId, [FromBody] UpdatePostDTO post)
        {
            if (!JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, userId))
            {
                return Forbid();
            }

            await _postService.UpdatePostAsync(post, userId, postId);
            return Ok();
        }
    }
}
