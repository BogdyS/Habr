using Habr.BusinessLogic.Validation;
using Habr.Common.Exceptions;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Servises
{
    public class UserService
    {
        public async Task<User> Login(string email, string password)
        {
            using (var context = new DataContext())
            {
                if (!await IsEmailTakenAsync(email, context))
                {
                    throw new LoginException("Email is incorrect");
                }

                User user = await context.Users
                    .SingleAsync(u => u.Email.Equals(email));

                if (user.Password != password)
                {
                    throw new LoginException("Wrong Email or password");
                }

                return user;
            }
        }
        public async Task Register(string name, string email, string password)
        {
            using (var context = new DataContext())
            {
                if (!EmailValidation.IsValidEmail(email))
                {
                    throw new LoginException("Email is not valid");
                }

                if (await IsEmailTakenAsync(email, context))
                {
                    throw new LoginException("Email is already taken");
                }

                await context.Users.AddAsync(new User()
                {
                    Email = email,
                    Password = password,
                    Name = name
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmailTakenAsync(string email, DataContext context)
        {
            return await context.Users.AnyAsync(x => x.Email == email);
        }
    }
}