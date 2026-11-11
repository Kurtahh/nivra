using backend.Models;

namespace backend.Interfaces;

public interface ILeaderboardService
{
    Task<List<LeaderboardEntry>> GetLeaderboardAsync(
        TimePeriod period = TimePeriod.Weekly,
        int topCount = 10);
}
