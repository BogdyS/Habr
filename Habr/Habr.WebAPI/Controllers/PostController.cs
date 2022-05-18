using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FullPostDTO>> GetPostAsync(int id)
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

        [HttpGet("drafts/{userId:int}")]
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

        [HttpGet("posts/{userId:int}")]
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

        [HttpPatch("drafts/public-draft")]
        public async Task<ActionResult> PublicPostFromDrafts([FromQuery] int userId, [FromQuery] int postId)
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

        [HttpPatch("drafts/to-drafts")]
        public async Task<ActionResult> RemovePostToDrafts([FromQuery] int userId, [FromQuery] int postId)
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

        [HttpPost("posts/create")]
        public async Task<ActionResult> CreatePostAsync([FromBody] CreatingPostDTO post)
        {
            try
            {
                int newPostId = await _postService.CreatePostAsync(post);
                return Created($"api/Post/{newPostId}", newPostId);
            }
            catch (InputException exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

        [HttpDelete("posts/delete")]
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

        [HttpPut("post/update")]
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
