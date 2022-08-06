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
    public class PostV2Controller : HabrController
    {
        private readonly IPostService _postService;

        public PostV2Controller(IPostService postService)
        {
            _postService = postService;
        }

        [AllowAnonymous]
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsV2Async());
        }
    }
}
