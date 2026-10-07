using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class LeaderboardRepo
{
    private readonly AppDbContext _context;

    public LeaderboardRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaderboardEntry>> GetTopWalkersAsync(DateOnly startDate, int topCount)
    {
        var rawGrouped = await _context.StepEntries
            .Where(entry => entry.Date >= startDate)
            .GroupBy(entry => entry.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                TotalSteps = group.Sum(step => step.StepCount)
            })
            .OrderByDescending(result => result.TotalSteps)
            .Take(topCount)
            .ToListAsync();

        var userIds = rawGrouped.Select(result => (int)result.UserId).ToList();

        var users = await _context.Users
            .Where(user => userIds.Contains(user.Id))
            .ToDictionaryAsync(user => (long)user.Id, user => user.Username);

        var entries = new List<LeaderboardEntry>();
        int rank = 1;

        foreach (var result in rawGrouped)
        {
            string username;

			if (users.TryGetValue(result.UserId, out var name))
			{
				username = name;
			}
			else 
			{
				username = "Unknown";
			}

            entries.Add(new LeaderboardEntry(result.UserId, username, result.TotalSteps, rank++));
        }

        return entries;
    }
}