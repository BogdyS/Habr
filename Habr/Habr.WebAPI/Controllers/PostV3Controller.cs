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
        }

        private readonly IConfiguration _configuration;

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public override Task<IActionResult> GetAllPostsAsync()
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPostsAsync([FromQuery] int pageNumber)
        {
            int pageSize = int.Parse(_configuration["Posts:PageSize"]);
            var postPage =await _postService.GetAllPostsPageAsync(pageNumber, pageSize);
            return Ok(postPage);
        }
    }
}
