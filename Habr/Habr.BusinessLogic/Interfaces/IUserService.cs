using Habr.Common.DTO;
using Habr.Common.DTO.User;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginDTO loginData);
    Task<UserDTO> GetUserAsync(int userId); 
    Task<UserDTO> RegisterAsync(RegistrationDTO newUser);
    Task<TokenResponse> RefreshTokensAsync(RefreshDTO refreshDto);
    Task<User?> IsUserExistsAsync(int userId);
}