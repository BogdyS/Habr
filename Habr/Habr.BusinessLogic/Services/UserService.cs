using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Validation;
using Habr.Common.DTO.User;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class UserService : IUserService
    {
        private readonly DataContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(DataContext dataContext, IMapper mapper)
        {
            _dbContext = dataContext;
            _mapper = mapper;
        }

        public async Task<UserDTO> LoginAsync(string email, string password)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null)
            {
                throw new LoginException("Email is incorrect");
            }

            if (user.Password != password)
            {
                throw new LoginException("Wrong Email or password");
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
                throw new SQLException("This user doesn't exists");
            }

            return user;
        }

        public async Task RegisterAsync(string name, string email, string password)
        {
            if (!UserValidation.IsValidEmail(email))
            {
                throw new LoginException("Email is not valid");
            }

            if (!UserValidation.IsValidPassword(password))
            {
                throw new LoginException("Password is not valid");
            }

            if (name.Length != 0)
            {
                throw new LoginException("Name is required");
            }

            if (await IsEmailExistsAsync(email))
            {
                throw new LoginException("Email is already taken");
            }

            _dbContext.Users.Add(new User()
            {
                Email = email,
                Password = password,
                Name = name
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserExistsAsync(int userId)
        {
            return await _dbContext.Users.AnyAsync(user => user.Id == userId);
        }

        private async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
        }
    }
}