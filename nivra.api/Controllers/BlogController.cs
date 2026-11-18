using backend.Models.Requests;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogService _service;

        public BlogController(BlogService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var posts = _service.GetAllPosts();
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] CreateBlogPostRequest request)
        {
            try
            {
                _service.CreatePost(request);
                return Ok("Post created successfully.");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}