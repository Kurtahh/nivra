using backend.Enums;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(IChatService service) : ControllerBase, IChatController
{
    [HttpPost]
    [Route("SendMessage")]
    public IActionResult SendUserMessage(Chat chat)
    {
        var status = service.SendMessageToChat(chat);

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
            messages = await service.GetMessagesAsync();
        }

        if (count != null)
        {
             
            messages = await service.GetMessagesAsync(count.Value);
        }

        return Ok(messages);

    }
}
