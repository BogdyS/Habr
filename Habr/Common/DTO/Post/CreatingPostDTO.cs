namespace Habr.Common.DTO;

public class CreatingPostDTO : IPostDTO
{
    public string? Title { get; set; }
    public string? Text { get; set; }
    public bool IsDraft { get; set; } 
    public int UserId { get; set; }
}