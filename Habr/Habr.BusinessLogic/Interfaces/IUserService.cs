using Habr.Common.DTO.User;

namespace Habr.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<UserDTO> LoginAsync(LoginDTO loginData);
    Task<UserDTO> GetUserAsync(int userId); 
    Task<int> RegisterAsync(RegistrationDTO newUser);
    Task<bool> IsUserExistsAsync(int userId);
}