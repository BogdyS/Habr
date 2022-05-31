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
            try
            {
                var user = await _userService.GetUserAsync(id);
                return Ok(user);
            }
            catch (NotFoundException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpGet("users/login")]
        public async Task<IActionResult> LoginAsync([FromQuery] LoginDTO loginData)
        {
            try
            {
                var user = await _userService.LoginAsync(loginData);
                return Ok(user);
            }
            catch (LoginException exception)
            {
                return NotFound(exception.ToDto());
            }
        }

        [HttpPost("users/registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegistrationDTO newUser)
        {
            try
            {
                var user = await _userService.RegisterAsync(newUser);
                return CreatedAtAction(nameof(GetUserAsync), new { id = user.Id }, user);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.ToDto());
            }
        }
    }
}
