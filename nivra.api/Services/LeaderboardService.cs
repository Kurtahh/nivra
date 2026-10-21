using backend.Models;
using backend.Repositories;
using backend.Extensions;

namespace backend.Services;

public class LeaderboardService
{
    private readonly LeaderboardRepo _leaderboardRepo;

    public LeaderboardService(LeaderboardRepo leaderboardRepo)
    {
        _leaderboardRepo = leaderboardRepo;
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(
        TimePeriod period = TimePeriod.Weekly, 
        int topCount = 10)
    {
        DateOnly startDate = DateUtils.GetStartDate(period);
        return await _leaderboardRepo.GetTopWalkersAsync(startDate, topCount);
    }
}