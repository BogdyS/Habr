namespace Habr.DataAccess.Entities
{
    public class Post
    {
        public Post()
        {
            Comments = new List<Comment>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsDraft { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
