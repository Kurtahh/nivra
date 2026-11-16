using backend.Enums;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(IBackgroundTaskQueue queue, IChatService service) : ControllerBase, IChatController
{
    [HttpPost]
    [Route("SendMessage")]
    public IActionResult SendUserMessage(Chat chat)
    {
        queue.Enqueue(chat);
        return Accepted(chat);

    }

    [HttpGet]
    [Route("Messages")]
    public async Task<IActionResult> GetMessages([FromQuery] int? count)
    {
        List<Chat> messages = [];
        if (count == null)
        {
            messages = await service.GetMessagesAsync();
        }

        if (count != null)
        {
             
            messages = await service.GetMessagesAsync(count.Value);
        }

        return Ok(messages);

    }
}
