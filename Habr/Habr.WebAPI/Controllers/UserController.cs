using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/user-management")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService, IPostService postService)
        {
            _userService = userService;
        }

        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] int id)
        {
            var user = await _userService.GetUserAsync(id);
            return Ok(user);
        }

        [HttpGet("users/login")]
        public async Task<IActionResult> LoginAsync([FromQuery] LoginDTO loginData)
        {
            var user = await _userService.LoginAsync(loginData);
            return Ok(user);
        }

        [HttpPost("users/registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegistrationDTO newUser)
        {
            var user = await _userService.RegisterAsync(newUser);
            return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
        }
    }
}
