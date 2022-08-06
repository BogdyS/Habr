namespace Habr.Common.DTO;

public class FullPostDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string AuthorEmail { get; set; }
    public DateTime PublishDate { get; set; }
    public IEnumerable<CommentDTO> Comments { get; set; }
}