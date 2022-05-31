using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/comment-management")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("comments")]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentDTO comment)
        {
            var createdComment = await _commentService.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetCommentsAsync), new { id = createdComment.Id }, createdComment);
        }

        [HttpGet("comments/{commentId:int}")]
        public async Task<IActionResult> GetCommentsAsync([FromRoute] int commentId)
        {
            var comment = await _commentService.GetCommentAsync(commentId);
            return Ok(comment);
        }

        [HttpDelete("users/{userId:int}/comments/{commentId:int}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int userId, [FromRoute] int commentId)
        {
            await _commentService.DeleteCommentAsync(commentId, userId);
            return Ok();
        }
    }
}
