using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface ILeaderboardController
{
    Task<IActionResult> GetLeaderboard(
        TimePeriod period = TimePeriod.Weekly,
        int top = 10);
}
