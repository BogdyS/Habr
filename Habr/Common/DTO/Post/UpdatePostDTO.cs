namespace Habr.Common.DTO;

public class UpdatePostDTO : IPost
{
    public string? Title { get; set; }
    public string? Text { get; set; }
}