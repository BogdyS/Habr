namespace Habr.Common.DTO;

public class UpdatePostDTO : IPost
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
}