using Habr.Common.DTO.User;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<UserDTO> LoginAsync(LoginDTO loginData);
    Task<UserDTO> GetUserAsync(int userId); 
    Task<UserDTO> RegisterAsync(RegistrationDTO newUser);
    Task<User?> IsUserExistsAsync(int userId);
}