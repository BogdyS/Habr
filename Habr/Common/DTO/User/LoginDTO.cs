namespace Habr.Common.DTO.User;

public class LoginDTO : IUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}