namespace Habr.Common.DTO;

public class UpdatePostDTO : IPostDTO
{
    public string? Title { get; set; }
    public string? Text { get; set; }
}