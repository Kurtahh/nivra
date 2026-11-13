using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface ISkinController
{
    IActionResult GetSkins();
    IActionResult GetSkinById(int id);
    IActionResult CreateSkin(Skin skin);
    IActionResult UpdateSkin(int id, Skin skin);
    IActionResult DeleteSkin(int id);
}
