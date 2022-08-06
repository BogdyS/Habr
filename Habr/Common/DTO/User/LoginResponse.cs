namespace Habr.Common.DTO.User;

public class LoginResponse
{
    public UserDTO User { get; set; }
    public TokenResponse Tokens { get; set; }
}