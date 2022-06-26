namespace Habr.DataAccess.Entities
{
    public class User
    {
        public User()
        {
            Posts = new List<Post>();
            Comments = new List<Comment>();
        }
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenActiveTo { get; set; }
        public RolesEnum Role { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}