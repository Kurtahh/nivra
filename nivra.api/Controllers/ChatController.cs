using backend.Enums;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly ChatService _service;

    public ChatController(ChatService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("SendMessage")]
    public IActionResult SendUserMessage(Chat chat)
    {
        var status = _service.SendMessageToChat(chat);

        if (status == SendStatus.MessageAbove200)
        {
            return BadRequest("Message is above 200!");
        }

        if (status == SendStatus.MessageIsEmpty)
        {
            return BadRequest("Message is empty");
        }

        if (status == SendStatus.MessageAlreadyExists)
        {
            return BadRequest("Message already exists");
        }

        return Ok("Message sent successfully!");

    }

    [HttpGet]
    [Route("Messages")]
    public async Task<IActionResult> GetMessages([FromQuery] int? count)
    {
        List<Chat> messages = [];
        if (count == null)
        {
            messages = await _service.GetMessagesAsync();
        }

        if (count != null)
        {
             
            messages = await _service.GetMessagesAsync(count.Value);
        }

        return Ok(messages);

    }
}
