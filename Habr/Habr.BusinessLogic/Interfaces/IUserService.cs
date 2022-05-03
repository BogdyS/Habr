using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<User> LoginAsync(string email, string password);
    Task RegisterAsync(string name, string email, string password);
}