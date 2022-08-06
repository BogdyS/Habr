using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.DTO.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("3")]
    [Route("api/v{version:apiVersion}/post")]
    [Route("api/post")]
    [Tags("Post")]
    public class PostV3Controller : HabrController
    {
        private readonly IPagedPostService _postService;
        private readonly int _pageSize;

        public PostV3Controller(IPagedPostService postService,
            IConfiguration configuration, ILogger<PostV3Controller> logger)
        {
            _postService = postService;
            _pageSize = int.Parse(configuration["Posts:PageSize"]);
        }

        [HttpGet("posts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPostsAsync([FromQuery] int pageNumber)
        {
            return Ok(await _postService.GetAllPostsPageAsync(pageNumber, _pageSize));
        }

        [HttpGet("users/{userId:int}/posts")]
        public async Task<IActionResult> GetUserPostsAsync([FromRoute] int userId, [FromQuery] int pageNumber)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserPostsPageAsync(userId, pageNumber, _pageSize));
        }

        [HttpGet("users/{userId:int}/posts/drafts")]
        public async Task<IActionResult> GetUserDraftsAsync([FromRoute] int userId, [FromQuery] int pageNumber)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserDraftsPageAsync(userId, pageNumber, _pageSize));
        }

    }
}
