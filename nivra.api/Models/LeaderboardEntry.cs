namespace backend.Models;

public record LeaderboardEntry(
    long UserId,
    string Username,
    int TotalSteps,
    int Rank
);