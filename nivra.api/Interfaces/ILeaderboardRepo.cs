using backend.Models;

namespace backend.Interfaces;

public interface ILeaderboardRepo
{
    Task<List<LeaderboardEntry>> GetTopWalkersAsync(DateOnly startDate, int topCount);
}
