using backend.Interfaces;
using backend.Models;
using backend.Repositories;
using backend.Extensions;

namespace backend.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly ILeaderboardRepo _leaderboardRepo;

    public LeaderboardService(ILeaderboardRepo leaderboardRepo)
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