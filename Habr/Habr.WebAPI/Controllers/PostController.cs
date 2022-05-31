using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/post-management")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpPost("posts")]
        public async Task<ActionResult> CreatePostAsync([FromBody] CreatingPostDTO post)
        {
            try
            {
                var newPost = await _postService.CreatePostAsync(post);
                return CreatedAtAction(nameof(GetPostAsync), new { id = newPost.Id }, newPost);
            }
            catch (InputException exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

        [HttpGet("posts/{postId:int}")]
        public async Task<ActionResult<FullPostDTO>> GetPostAsync([FromRoute] int postId)
        {
            try
            {
                var post = await _postService.GetPostWithCommentsAsync(postId);
                return Ok(post);
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpGet("users/{userId:int}/posts")]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetUserPostsAsync([FromRoute] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserPostsAsync(userId));
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpGet("users/{userId:int}/posts/drafts")]
        public async Task<ActionResult<IEnumerable<PostDraftDTO>?>> GetUserDraftsAsync([FromRoute] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserDraftsAsync(userId));
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/public-from-drafts")]
        public async Task<ActionResult> PublicPostFromDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            try
            {
                await _postService.PostFromDraftAsync(postId, userId);
                return Ok();
            }
            catch (Exception exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpPatch("users/{userId:int}/posts/{postId:int}/remove-to-drafts")]
        public async Task<ActionResult> RemovePostToDraftsAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            try
            {
                await _postService.RemovePostToDraftsAsync(postId, userId);
                return Ok();
            }
            catch (Exception exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpDelete("users/{userId:int}/posts/{postId:int}")]
        public async Task<ActionResult> DeletePostAsync([FromRoute] int userId, [FromRoute] int postId)
        {
            try
            {
                await _postService.DeletePostAsync(postId, userId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

        [HttpPut("users/{userId:int}/posts/{postId:int}")]
        public async Task<ActionResult> UpdatePostAsync([FromRoute] int userId, [FromRoute] int postId, [FromBody] UpdatePostDTO post)
        {
            try
            {
                await _postService.UpdatePostAsync(post.Title, post.Text, postId, userId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }
    }
}
