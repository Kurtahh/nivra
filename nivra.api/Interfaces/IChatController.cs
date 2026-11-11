using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IChatController
{
    IActionResult SendUserMessage(Chat chat);
    Task<IActionResult> GetMessages(int? count);
}
