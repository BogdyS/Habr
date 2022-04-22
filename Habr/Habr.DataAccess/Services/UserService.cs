using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Servises
{
    public class UserService
    {
        public async Task<User> Login(string email, string password)
        {
            using (var context = new DataContext())
            {
                try
                {
                    User user = await context.Users
                        .SingleAsync(u => u.Email.Equals(email) && u.Password.Equals(password));
                    return user;
                }
                catch (InvalidOperationException exception)
                {
                    throw new LoginException(exception.Message);
                }
            }
        }

        public async Task Register(string name, string email, string password)
        {
            using (var context = new DataContext())
            {
                if (await context.Users.AnyAsync(x => x.Email == email))
                {
                    throw new LoginException("Email is not unique");
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
    }
}