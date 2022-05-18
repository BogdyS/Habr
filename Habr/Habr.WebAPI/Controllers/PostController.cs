using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private DataContext _dataContext;
        private readonly IMapper _mapper;

        public PostController(IPostService postService, ICommentService commentService, DataContext dataContext, IMapper mapper)
        {
            _postService = postService;
            _commentService = commentService;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FullPostDTO>> GetPostByIdAsync(int id)
        {
            try
            {
                var post = await _postService.GetPostWithCommentsAsync(id);
                return Ok(post);
            }
            catch (SQLException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet("user/drafts")]
        public async Task<ActionResult<IEnumerable<PostDraftDTO>?>> GetDraftsAsync([FromQuery] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserDraftsAsync(userId));
            }
            catch (SQLException exception)
            {
                return NotFound(exception.Message);
            }
        }
        [HttpGet("user/posts")]
        public async Task<ActionResult<IEnumerable<PostListDTO>?>> GetUserPostsAsync([FromQuery] int userId)
        {
            try
            {
                return Ok(await _postService.GetUserPostsAsync(userId));
            }
            catch (SQLException exception)
            {
                return NotFound(exception.Message);
            }
        }

    }
}
