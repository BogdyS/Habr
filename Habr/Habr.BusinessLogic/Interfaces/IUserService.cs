using Habr.Common.DTO.User;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<UserDTO> LoginAsync(string email, string password);
    Task<UserDTO> GetUserAsync(int userId); 
    Task RegisterAsync(string name, string email, string password);
    Task<bool> IsUserExistsAsync(int userId);
}