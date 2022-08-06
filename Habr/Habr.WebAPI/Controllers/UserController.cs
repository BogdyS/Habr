using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.WebAPI.BackgroundJobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [ApiVersion("2")]
    [ApiVersion("3")]
    [Route("api/v{version:apiVersion}/user")]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, ILogger<UserController> logger, IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
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
            var response = await _userService.LoginAsync(loginData);

            _logger.LogInformation($"Successful login user login = {loginData.Login}");

            return Ok(response);
        }

        [HttpPost("users/refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshDTO dto)
        {
            var response = await _userService.RefreshTokensAsync(dto);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("users/exit")]
        public async Task<IActionResult> ExitAsync()
        {
            int userId = JwtHelper.GetClaimUserId(HttpContext.User.Claims);
            await _userService.ExitAsync(userId);
            return Ok();
        }

        [HttpPost("users/registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegistrationDTO newUser,
            [FromServices] IRecurringJobManager jobManager, [FromServices] IEmailSender emailSender)
        {
            var user = await _userService.RegisterAsync(newUser);

            _logger.LogInformation($"Created new user with id = {user.Id}");

            return CreatedAtAction(nameof(LoginAsync),
                new LoginDTO()
                {
                    Login = newUser.Login,
                    Password = newUser.Password
                },
                user);
        }
    }
}
