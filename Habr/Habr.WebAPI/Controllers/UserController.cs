using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, ILogger<UserController> logger, IJwtService jwtService, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _jwtService = jwtService;
            _mapper = mapper;
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

            var token = _jwtService.GetJwt(user);

            _logger.LogInformation($"Successful login user login = {loginData.Login}");

            return Ok(token);
        }

        [HttpPost("users/registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegistrationDTO newUser)
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
