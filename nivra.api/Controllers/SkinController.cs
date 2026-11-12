using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkinController : ControllerBase, ISkinController
{
    private readonly ISkinService _service;

    public SkinController(ISkinService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSkins()
    {
        return Ok(_service.GetSkins());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetSkinById(int id)
    {
        var skin = _service.GetSkinById(id);
        if (skin == null)
        {
            return NotFound(new { message = "Skin not found." });
        }

        return Ok(skin);
    }

    [HttpPost]
    public IActionResult CreateSkin([FromBody] Skin skin)
    {
        try
        {
            var createdSkin = _service.CreateSkin(skin);
            return CreatedAtAction(nameof(GetSkinById), new { id = createdSkin.Id }, createdSkin);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateSkin(int id, [FromBody] Skin skin)
    {
        try
        {
            var updatedSkin = _service.UpdateSkin(id, skin);
            if (updatedSkin == null)
            {
                return NotFound(new { message = "Skin not found." });
            }

            return Ok(updatedSkin);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteSkin(int id)
    {
        if (!_service.DeleteSkin(id))
        {
            return NotFound(new { message = "Skin not found." });
        }

        return NoContent();
    }
}
