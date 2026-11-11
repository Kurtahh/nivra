using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IHealthTipController
{
    IActionResult GetRandomTip();
}
