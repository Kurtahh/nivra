using backend.Enums;

namespace backend.Models.Requests
{
    public class CreateBlogPostRequest
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public BlogCategory Category { get; set; }
        public int UserId { get; set; }
    }
}