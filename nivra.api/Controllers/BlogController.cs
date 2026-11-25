using backend.Models.Requests;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var posts = _blogService.GetAllPosts();
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] CreateBlogPostRequest request)
        {
            try
            {
                _blogService.CreatePost(request);
                return Ok("Post created successfully.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId, [FromQuery] int userId)
        {
            bool deleted = _blogService.DeletePost(postId, userId);

            if (!deleted)
                return NotFound("Post not found or you cannot delete it.");

            return Ok("Post deleted successfully.");
        }
    }
}