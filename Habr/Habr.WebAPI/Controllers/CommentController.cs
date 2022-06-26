﻿using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/comment")]
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
            JwtHelper.IsJwtIdClaimValid(HttpContext.User.Claims, comment.UserId);

            var createdComment = await _commentService.CreateCommentAsync(comment);
            return CreatedAtAction(nameof(GetCommentsAsync), new { commentId = createdComment.Id }, createdComment);
        }

        [HttpGet("comments/{commentId:int}")]
        public async Task<IActionResult> GetCommentsAsync([FromRoute] int commentId)
        {
            var comment = await _commentService.GetCommentAsync(commentId);
            return Ok(comment);
        }

        [HttpPut("users/{userId:int}/comments/{commentId:int}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int userId, [FromRoute] int commentId, [FromBody] UpdateCommentDTO commentDto)
        {
            var claims = HttpContext.User.Claims;

            JwtHelper.IsJwtIdClaimValid(claims, userId);

            var role = JwtHelper.GetClaimRole(claims);

            await _commentService.UpdateCommentAsync(commentDto.Text!, commentId, userId, role);
            return Ok();
        }

        [HttpDelete("users/{userId:int}/comments/{commentId:int}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int userId, [FromRoute] int commentId)
        {
            var claims = HttpContext.User.Claims;

            JwtHelper.IsJwtIdClaimValid(claims, userId);

            var role = JwtHelper.GetClaimRole(claims);

            await _commentService.DeleteCommentAsync(commentId, userId, role);
            return Ok();
        }
    }
}
