namespace Habr.DataAccess.Entities
{
    public class Comment
    {
        public Comment()
        {
            Comments = new List<Comment>();
        }
        public int Id { get; set; }
        public string Text { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime Created { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}