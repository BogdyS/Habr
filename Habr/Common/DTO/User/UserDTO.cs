namespace Habr.Common.DTO.User;

public class UserDTO
{
    public RolesEnum Role { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Id { get; set; }
    public DateTime DateOfBirth { get; set; }
}