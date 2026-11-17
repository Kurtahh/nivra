using backend.Controllers;
using backend.Extensions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace nivra.api.Tests.Leaderboard;

public class LeaderboardTests
{
    private static void Seed(AppDbContext context)
    {
        context.Users.AddRange(
            new User { Id = 1, Username = "alice", PasswordHash = "hash" },
            new User { Id = 2, Username = "bob", PasswordHash = "hash" });
        context.StepEntries.AddRange(
            new StepEntry { Id = 1, UserId = 1, Date = new DateOnly(2026, 10, 1), StepCount = 1000 },
            new StepEntry { Id = 2, UserId = 2, Date = new DateOnly(2026, 10, 2), StepCount = 3000 },
            new StepEntry { Id = 3, UserId = 1, Date = new DateOnly(2026, 10, 3), StepCount = 500 },
            new StepEntry { Id = 4, UserId = 3, Date = new DateOnly(2026, 10, 3), StepCount = 2000 },
            new StepEntry { Id = 5, UserId = 2, Date = new DateOnly(2026, 9, 1), StepCount = 9999 });
        context.SaveChanges();
    }

    [Fact]
    public async Task Repository_AggregatesStepsAndRanksUsers()
    {
        var context = TestDb.Create();
        Seed(context);
        var repository = new LeaderboardRepo(context);

        var entries = await repository.GetTopWalkersAsync(new DateOnly(2026, 10, 1), 10);

        Assert.Equal(3, entries.Count);
        Assert.Equal(1, entries[0].Rank);
        Assert.Equal(2, entries[0].UserId);
        Assert.Equal("bob", entries[0].Username);
        Assert.Equal(3000, entries[0].TotalSteps);
        Assert.Equal(2, entries[1].Rank);
        Assert.Equal(3, entries[1].UserId);
        Assert.Equal("Unknown", entries[1].Username);
        Assert.Equal(3, entries[2].Rank);
        Assert.Equal(1, entries[2].UserId);
        Assert.Equal("alice", entries[2].Username);
        Assert.Equal(1500, entries[2].TotalSteps);
    }

    [Fact]
    public async Task Repository_IgnoresEntriesBeforeTheStartDate()
    {
        var context = TestDb.Create();
        Seed(context);
        var repository = new LeaderboardRepo(context);

        var entries = await repository.GetTopWalkersAsync(new DateOnly(2026, 10, 2), 10);

        Assert.DoesNotContain(entries, entry => entry.UserId == 2 && entry.TotalSteps == 12999);
        Assert.Equal(3000, entries.Single(entry => entry.UserId == 2).TotalSteps);
    }

    [Fact]
    public async Task Repository_ReturnsOnlyTheRequestedNumberOfUsers()
    {
        var context = TestDb.Create();
        Seed(context);
        var repository = new LeaderboardRepo(context);

        var entries = await repository.GetTopWalkersAsync(DateOnly.MinValue, 2);

        Assert.Equal(2, entries.Count);
        Assert.Equal(new[] { 1, 2 }, entries.Select(entry => entry.Rank));
    }

    [Fact]
    public async Task Repository_ReturnsEmptyListWhenThereAreNoSteps()
    {
        var repository = new LeaderboardRepo(TestDb.Create());

        var entries = await repository.GetTopWalkersAsync(DateOnly.MinValue, 10);

        Assert.Empty(entries);
    }

    [Theory]
    [InlineData(TimePeriod.Daily)]
    [InlineData(TimePeriod.Weekly)]
    [InlineData(TimePeriod.Monthly)]
    [InlineData(TimePeriod.AllTime)]
    public async Task Service_UsesTheStartDateForTheRequestedPeriod(TimePeriod period)
    {
        var repository = new Mock<ILeaderboardRepo>();
        repository
            .Setup(repo => repo.GetTopWalkersAsync(It.IsAny<DateOnly>(), It.IsAny<int>()))
            .ReturnsAsync([]);
        var service = new LeaderboardService(repository.Object);

        await service.GetLeaderboardAsync(period, 5);

        repository.Verify(
            repo => repo.GetTopWalkersAsync(DateUtils.GetStartDate(period), 5),
            Times.Once);
    }

    [Fact]
    public async Task Service_UsesWeeklyPeriodAndTopTenByDefault()
    {
        var repository = new Mock<ILeaderboardRepo>();
        repository
            .Setup(repo => repo.GetTopWalkersAsync(It.IsAny<DateOnly>(), It.IsAny<int>()))
            .ReturnsAsync([]);
        var service = new LeaderboardService(repository.Object);

        await service.GetLeaderboardAsync();

        repository.Verify(
            repo => repo.GetTopWalkersAsync(DateUtils.GetStartDate(TimePeriod.Weekly), 10),
            Times.Once);
    }

    [Fact]
    public async Task Controller_ReturnsOkWithTheLeaderboard()
    {
        var context = TestDb.Create();
        Seed(context);
        var service = new LeaderboardService(new LeaderboardRepo(context));
        var controller = new LeaderboardController(service);

        var result = await controller.GetLeaderboard(TimePeriod.AllTime, 10);

        var ok = Assert.IsType<OkObjectResult>(result);
        var entries = Assert.IsAssignableFrom<List<LeaderboardEntry>>(ok.Value);
        Assert.NotEmpty(entries);
    }

    [Fact]
    public void DateUtils_ReturnsStartDatesForEveryPeriod()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.Equal(today, DateUtils.GetStartDate(TimePeriod.Daily));
        Assert.Equal(today.AddDays(-7), DateUtils.GetStartDate(TimePeriod.Weekly));
        Assert.Equal(today.AddMonths(-1), DateUtils.GetStartDate(TimePeriod.Monthly));
        Assert.Equal(DateOnly.MinValue, DateUtils.GetStartDate(TimePeriod.AllTime));
    }

    [Fact]
    public void DateUtils_FallsBackToWeeklyForAnUnknownPeriod()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        Assert.Equal(today.AddDays(-7), DateUtils.GetStartDate((TimePeriod)999));
    }
}
