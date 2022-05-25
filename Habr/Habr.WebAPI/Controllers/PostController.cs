using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FullPostDTO>> GetPostAsync([FromRoute] int id)
        {
            try
            {
                var post = await _postService.GetPostWithCommentsAsync(id);
                return Ok(post);
            }
            catch (SQLException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpGet("user/drafts")]
        public async Task<ActionResult<IEnumerable<PostDraftDTO>?>> GetUserDraftsAsync([FromQuery] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserDraftsAsync(userId));
            }
            catch (SQLException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetUserPostsAsync([FromQuery] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserPostsAsync(userId));
            }
            catch (SQLException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpPatch("user/drafts/public")]
        public async Task<ActionResult> PublicPostFromDraftsASync([FromQuery] int userId, [FromQuery] int postId)
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

        [HttpPatch("user/drafts/remove")]
        public async Task<ActionResult> RemovePostToDraftsAsync([FromQuery] int userId, [FromQuery] int postId)
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

        [HttpPost()]
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

        [HttpDelete()]
        public async Task<ActionResult> DeletePostAsync([FromQuery] int userId, [FromQuery] int postId)
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

        [HttpPut()]
        public async Task<ActionResult> UpdatePostAsync([FromQuery] int userId, [FromQuery] int postId, [FromBody] UpdatePostDTO post)
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
