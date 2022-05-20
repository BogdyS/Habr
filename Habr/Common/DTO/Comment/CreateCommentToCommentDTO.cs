namespace Habr.Common.DTO;

public class CreateCommentToCommentDTO
{
    public string? Text { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int ParentCommentId { get; set; }
}