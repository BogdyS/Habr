using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Validation;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class UserService : IUserService
    {
        private readonly DataContext _dbContext;

        public UserService(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            User? user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null)
            {
                throw new LoginException("Email is incorrect");
            }

            if (user.Password != password)
            {
                throw new LoginException("Wrong Email or password");
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

            if (await IsEmailExistsAsync(email, _dbContext))
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

        private async Task<bool> IsEmailExistsAsync(string email, DataContext context)
        {
            return await context.Users.AnyAsync(x => x.Email == email);
        }
    }
}