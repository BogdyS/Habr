namespace Habr.Common.DTO;

public class CreateCommentDTO
{
    public string? Text { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
}