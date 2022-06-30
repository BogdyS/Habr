using System.ComponentModel;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/post")]
    [Route("api/post")]
    [Tags("Post")]
    public class PostV2Controller : PostV1Controller
    {
        public PostV2Controller(IPostService postService, ILogger<PostV1Controller> logger) : base(postService, logger) { }

        [AllowAnonymous]
        [HttpGet("posts")]
        public override async Task<IActionResult> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsV2Async());
        }
    }
}
