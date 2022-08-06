using Habr.Common.DTO.User;

namespace Habr.Common.DTO;

public class PostListDtoV2
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Posted { get; set; }
    public AuthorDto Author { get; set; }
}