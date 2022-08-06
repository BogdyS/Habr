namespace Habr.Common.DTO;

public class CreateCommentDTO : ICommentDTO
{
    public string? Text { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
}