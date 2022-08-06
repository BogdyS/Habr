namespace Habr.Common.DTO.User;

public interface IUserDTO
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}