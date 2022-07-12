using System.ComponentModel.DataAnnotations;

namespace Habr.DataAccess.Entities;

public class Rate
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }

    [Range(1,5)]
    public int Value { get; set; }

    public User User { get; set; }
    public Post Post { get; set; }
}