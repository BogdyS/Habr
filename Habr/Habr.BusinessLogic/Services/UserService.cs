using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.Common.Exceptions;
using Habr.Common.Resourses;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InvalidDataException = Habr.Common.Exceptions.InvalidDataException;

namespace Habr.BusinessLogic.Servises
{
    public class UserService : IUserService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<RegistrationDTO> _userValidator;
        private readonly IPasswordHasher<IUserDTO> _hasher;
        private readonly IJwtService _jwtService;
        public UserService(DataContext dataContext, IMapper mapper, IValidator<RegistrationDTO> userValidator, IPasswordHasher<IUserDTO> hasher, IJwtService jwtService)
        {
            _dbContext = dataContext;
            _mapper = mapper;
            _userValidator = userValidator;
            _hasher = hasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO loginData)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(loginData.Login));
            
            if (user == null)
            {
                throw new BusinessLogicException(ExceptionMessages.UserWithEmailNotFound);
            }

            if (_hasher.VerifyHashedPassword(loginData, user.Password, loginData.Password) == PasswordVerificationResult.Failed)
            {
                throw new BusinessLogicException(ExceptionMessages.LoginError);
            }

            var userResponse = _mapper.Map<UserDTO>(user);

            var tokenResponse = new TokenResponse()
            {
                AccessToken = _jwtService.GetJwt(userResponse),
                RefreshToken = _jwtService.GetRefreshToken()
            };

            user.RefreshToken = tokenResponse.RefreshToken;
            user.RefreshTokenActiveTo = _jwtService.RefreshTokenValidTo;

            await _dbContext.SaveChangesAsync();
            var response = new LoginResponse()
            {
                User = userResponse,
                Tokens = tokenResponse
            };

            return response;
        }

        public async Task<TokenResponse> RefreshTokensAsync(RefreshDTO refreshDto)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == int.Parse(refreshDto.UserId!));

            if (user == null)
            {
                throw new NotFoundException(ExceptionMessages.UserNotFound);
            }

            if (user.RefreshToken == null || !user.RefreshToken.Equals(refreshDto.RefreshToken) ||
                DateTime.UtcNow > user.RefreshTokenActiveTo)
            {
                throw new ForbiddenException(ExceptionMessages.RefreshTokenInvalid);
            }

            var userDto = _mapper.Map<UserDTO>(user);

            var accessToken = _jwtService.GetJwt(userDto);
            var refreshToken = _jwtService.GetRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenActiveTo = _jwtService.RefreshTokenValidTo;
            await _dbContext.SaveChangesAsync();

            return new TokenResponse()
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken
            };
        }

        public async Task ExitAsync(int userId)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new NotFoundException(ExceptionMessages.UserNotFound);
            }

            user.RefreshToken = null;
            user.RefreshTokenActiveTo = null;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUserAsync(int userId)
        {
            var user = await _dbContext.Users
                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(user => user.Id == userId);

            if (user == null)
            {
                throw new NotFoundException(ExceptionMessages.UserNotFound);
            }

            return user;
        }

        public async Task<UserDTO> RegisterAsync(RegistrationDTO newUser)
        {
            var validationResult = await _userValidator.ValidateAsync(newUser);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.First();
                string attemptedValue;
                if (error.AttemptedValue is DateTime)
                {
                    attemptedValue = error.AttemptedValue.ToString();
                }
                else
                {
                    attemptedValue = (string) error.AttemptedValue;
                }
                throw new InvalidDataException(error.ErrorMessage, attemptedValue);
            }

            if (await IsEmailExistsAsync(newUser.Login))
            {
                throw new BusinessLogicException(ExceptionMessages.EmailTaken);
            }

            newUser.Password = _hasher.HashPassword(newUser, newUser.Password);
            var user = _mapper.Map<User>(newUser);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<User?> IsUserExistsAsync(int userId)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
        }

        private async Task<bool> IsEmailExistsAsync(string? email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
        }
    }
}