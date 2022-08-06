namespace Habr.Common.DTO.User;

public class RegistrationDTO : IUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public DateTime DateOfBirth { get; set; }
}