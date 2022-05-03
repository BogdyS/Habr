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
        public async Task<User> LoginAsync(string email, string password)
        {
            using (var context = new DataContext())
            {
                User? user = await context.Users
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
        }

        public async Task RegisterAsync(string name, string email, string password)
        {
            using (var context = new DataContext())
            {
                if (!EmailValidation.IsValidEmail(email))
                {
                    throw new LoginException("Email is not valid");
                }

                if (await IsEmailExistsAsync(email, context))
                {
                    throw new LoginException("Email is already taken");
                }

                context.Users.Add(new User()
                {
                    Email = email,
                    Password = password,
                    Name = name
                });
                await context.SaveChangesAsync();
            }
        }

        private async Task<bool> IsEmailExistsAsync(string email, DataContext context)
        {
            return await context.Users.AnyAsync(x => x.Email == email);
        }
    }
}