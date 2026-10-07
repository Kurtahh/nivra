using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaderboardController : ControllerBase
{
    private readonly LeaderboardService _service;

    public LeaderboardController(LeaderboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboard([FromQuery] TimePeriod period = TimePeriod.Weekly, [FromQuery] int top = 10)
    {
        var leaderboard = await _service.GetLeaderboardAsync(period: period, topCount: top);
        return Ok(leaderboard);
    }
}