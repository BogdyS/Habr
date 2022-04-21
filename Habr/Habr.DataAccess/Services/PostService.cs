using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Servises
{
    public class PostService
    {
        public List<Post> GetAllPosts()
        {
            using (var context = new DataContext())
            {
                List<Post> posts = context.Posts.ToList();
                List<int> authorsId = new List<int>();
                foreach (var post in posts)
                {
                    if (!authorsId.Contains(post.UserId))
                    {
                        authorsId.Add(post.UserId);
                    }
                }
                foreach (int id in authorsId)
                {
                    context.Users.Where(u=>u.Id == id).Load();
                }
                return posts;
            }
        }
        public List<Post> GetUserPosts(User user)
        {
            using (var context = new DataContext()) 
            {
                context.Users.Where(u => u.Id == user.Id).Load();
                return context.Posts.Where(p => p.UserId == user.Id).ToList();
            }
        }
        public Post GetPost(int id)
        {
            using (var context = new DataContext())
            {
                try
                {
                    Post post = context.Posts.Single(p => p.Id == id);
                    context.Users.Where(u => u.Id == post.UserId).Load();
                    return post;
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
            }
        }
        public async Task CreatePost(Post post)
        {
            using (var context = new DataContext())
            {
                await context.Posts.AddAsync(post);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdatePost(Post post)
        {
            using (var context = new DataContext())
            {
                Post postToUpdate;
                try
                {
                    postToUpdate = context.Posts.Single(p => p.Id == post.Id);
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
                postToUpdate.Text = post.Text;
                postToUpdate.Title = post.Title;
                await context.SaveChangesAsync();
            }
        }
        public async Task DeletePost(int id)
        {
            using (var context = new DataContext())
            {
                Post post;
                try
                {
                    post = context.Posts.Single(p => p.Id == id);
                }
                catch (InvalidOperationException)
                {
                    throw new SQLException("Post not found");
                }
                context.Posts.Remove(post);
                await context.SaveChangesAsync();
            }
        }
    }
}