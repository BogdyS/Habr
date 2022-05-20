namespace Habr.Common.DTO;

public class CommentDTO
{
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public string Text { get; set; }
    public IEnumerable<CommentDTO>? Comments { get; set; }
}