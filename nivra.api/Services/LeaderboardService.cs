using backend.Models;
using backend.Repositories;

namespace backend.Services;

public class LeaderboardService
{
    private readonly LeaderboardRepo _repository;

    public LeaderboardService(LeaderboardRepo repository)
    {
        _repository = repository;
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboardAsync(
        TimePeriod period = TimePeriod.Weekly, 
        int topCount = 10)
    {
        DateOnly startDate = period switch
        {
            TimePeriod.Daily => DateOnly.FromDateTime(DateTime.UtcNow),
            TimePeriod.Weekly => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
            TimePeriod.Monthly => DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            TimePeriod.AllTime => DateOnly.MinValue,
            _ => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7))
        };

        return await _repository.GetTopWalkersAsync(startDate, topCount);
    }
}