using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
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
        public async Task<ActionResult<IEnumerable<PostListDTO>>> GetAllPostsAsync()
        {
            return Ok(await _postService.GetAllPostsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FullPostDTO>> GetPostByIdAsync(int id)
        {
            return Ok(await _postService.GetPostWithCommentsAsync(id));
        }

    }
}
