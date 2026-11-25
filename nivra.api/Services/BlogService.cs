using backend.Enums;
using backend.Models;
using backend.Models.Requests;
using backend.Repositories;

namespace backend.Services
{
    public class BlogService
    {
        private readonly BlogRepo _blogRepository;

        public BlogService(BlogRepo blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public List<BlogPost> GetAllPosts()
        {
            return _blogRepository.GetAll();
        }

        public void CreatePost(CreateBlogPostRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Post title cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Post cannot be empty.");

            if (!Enum.IsDefined(request.Category))
                throw new ArgumentException("Invalid blog category.");

            var user = _blogRepository.GetUser(request.UserId);

            if (user == null)
                throw new ArgumentException("User not found");

            var post = new BlogPost
            {
                Title = request.Title,
                Content = request.Content,
                Category = request.Category,
                UserId = user.Id,
                Username = user.Username
            };

            _blogRepository.Add(post);
        }
        public bool DeletePost(int postId, int userId)
        {
            return _blogRepository.Delete(postId, userId);
        }
    }
}