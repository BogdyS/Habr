using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Servises
{
    public class UserService
    {
        public User Login(string email, string password)
        {
            using (var context = new DataContext())
            {
                try
                {
                    User user = context.Users
                        .Single(u => u.Email.Equals(email) && u.Password.Equals(password));
                    return user;
                }
                catch (InvalidOperationException exception)
                {
                    throw new LoginException(exception.Message);
                }
            }
        }

        public async void Register(User newUser)
        {
            using (var context = new DataContext())
            {
                int users = context.Users
                    .Select(u => u.Email)
                    .Count(u => u == newUser.Email);
                if (users > 0)
                {
                    throw new LoginException("Email is not unique");
                }
                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();
            }
        }
    }
}