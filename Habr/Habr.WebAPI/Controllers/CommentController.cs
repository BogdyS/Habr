using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateCommentToPostAsync([FromBody] CreateCommentDTO comment)
        {
            try
            {
                int id = await _commentService.CreateCommentToPostAsync(comment);
                return Created("", id);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

        [HttpPost("/to-comment")]
        public async Task<IActionResult> CreateCommentToCommentAsync([FromBody] CreateCommentToCommentDTO comment)
        {
            try
            {
                int id = await _commentService.CreateCommentToCommentAsync(comment);
                return Created("", id);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteCommentAsync([FromQuery] int userId, [FromQuery] int commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(commentId, userId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }

    }
}
