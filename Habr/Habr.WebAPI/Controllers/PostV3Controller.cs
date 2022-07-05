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
    public class PostV3Controller : PostV2Controller
    {
        public PostV3Controller(IPostService postService, ILogger<PostV1Controller> logger,
            IConfiguration configuration) : base(postService, logger)
        {
            _configuration = configuration;
            _pageSize = int.Parse(_configuration["Posts:PageSize"]);
        }

        private readonly IConfiguration _configuration;
        private readonly int _pageSize;

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public override Task<IActionResult> GetAllPostsAsync()
        {
            throw new NotImplementedException();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public override Task<IActionResult> GetUserPostsAsync(int userId)
        {
            throw new NotImplementedException();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public override Task<IActionResult> GetUserDraftsAsync(int userId)
        {
            throw new NotImplementedException();
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
        public virtual async Task<IActionResult> GetUserDraftsAsync([FromRoute] int userId, [FromQuery] int pageNumber)
        {
            CheckClaimId(HttpContext.User.Claims, userId);

            return Ok(await _postService.GetUserDraftsPageAsync(userId, pageNumber, _pageSize));
        }

    }
}
