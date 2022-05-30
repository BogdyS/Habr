namespace Habr.Common.DTO;

public class PostListDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Posted { get; set; }
    public string UserEmail { get; set; }
}