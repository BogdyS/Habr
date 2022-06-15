using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTO.User;
using Habr.Common.Exceptions;
using Habr.Common.Resourses;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using InvalidDataException = Habr.Common.Exceptions.InvalidDataException;

namespace Habr.BusinessLogic.Servises
{
    public class UserService : IUserService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<RegistrationDTO> _userValidator;
        public UserService(DataContext dataContext, IMapper mapper, IValidator<RegistrationDTO> userValidator)
        {
            _dbContext = dataContext;
            _mapper = mapper;
            _userValidator = userValidator;
        }

        public async Task<UserDTO> LoginAsync(LoginDTO loginData)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(loginData.Login));

            if (user == null)
            {
                throw new BusinessLogicException(ExceptionMessages.UserWithEmailNotFound);
            }

            if (user.Password != loginData.Password)
            {
                throw new BusinessLogicException(ExceptionMessages.LoginError);
            }

            return _mapper.Map<UserDTO>(user);
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
                throw new InvalidDataException(error.ErrorMessage, (string) error.AttemptedValue);
            }

            if (await IsEmailExistsAsync(newUser.Login))
            {
                throw new BusinessLogicException(ExceptionMessages.EmailTaken);
            }

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