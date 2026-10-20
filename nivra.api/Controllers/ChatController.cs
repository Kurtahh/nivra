using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
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
        _service.SendMessageToChat(chat);
        return Ok("Message was sent successfully");
    }
}