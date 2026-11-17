using backend.Controllers;
using backend.Models;
using backend.Models.Requests;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace nivra.api.Tests.Steps;

public class StepsTests
{
    private static (StepsService Service, StepsRepo Repository, AppDbContext Context) CreateService()
    {
        var context = TestDb.Create();
        var repository = new StepsRepo(context);
        return (new StepsService(repository), repository, context);
    }

    [Fact]
    public void Repository_ReturnsEntryForTheGivenDateAndUser()
    {
        var context = TestDb.Create();
        var repository = new StepsRepo(context);
        var date = new DateOnly(2026, 10, 1);
        repository.Add(new StepEntry { Date = date, StepCount = 100, UserId = 1 });

        var entry = repository.EntryByDate(date, 1);

        Assert.NotNull(entry);
        Assert.Equal(100, entry.StepCount);
        Assert.Null(repository.EntryByDate(date, 2));
    }

    [Fact]
    public void Repository_UpdatesAnExistingEntry()
    {
        var context = TestDb.Create();
        var repository = new StepsRepo(context);
        var date = new DateOnly(2026, 10, 1);
        var entry = new StepEntry { Date = date, StepCount = 100, UserId = 1 };
        repository.Add(entry);

        entry.StepCount = 250;
        repository.Update(entry);

        Assert.Equal(250, repository.EntryByDate(date, 1)!.StepCount);
    }

    [Fact]
    public void Service_LogsStepsForTheFirstTime()
    {
        var (service, repository, _) = CreateService();
        var today = DateOnly.FromDateTime(DateTime.Now);

        service.LogSteps(100, 1);

        var entry = repository.EntryByDate(today, 1);
        Assert.NotNull(entry);
        Assert.Equal(100, entry.StepCount);
    }

    [Fact]
    public void Service_OverridesStepsWhenTheNewCountIsHigher()
    {
        var (service, repository, context) = CreateService();
        var today = DateOnly.FromDateTime(DateTime.Now);

        service.LogSteps(100, 1);
        service.LogSteps(250, 1);

        var entry = repository.EntryByDate(today, 1);
        Assert.NotNull(entry);
        Assert.Equal(250, entry.StepCount);
        Assert.Single(context.StepEntries);
    }

    [Fact]
    public void Service_RejectsANewCountThatDoesNotExceedTheStoredCount()
    {
        var (service, _, _) = CreateService();
        service.LogSteps(100, 1);

        Assert.Throws<ArgumentException>(() => service.LogSteps(100, 1));
        Assert.Throws<ArgumentException>(() => service.LogSteps(50, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Service_RejectsNonPositiveStepCounts(int stepCount)
    {
        var (service, _, _) = CreateService();

        Assert.Throws<ArgumentException>(() => service.LogSteps(stepCount, 1));
    }

    [Fact]
    public void Service_ReturnsTodaysSteps()
    {
        var (service, _, _) = CreateService();
        service.LogSteps(321, 5);

        Assert.Equal(321, service.GetTodaySteps(5));
    }

    [Fact]
    public void Service_ReturnsZeroWhenThereAreNoStepsToday()
    {
        var (service, _, _) = CreateService();

        Assert.Equal(0, service.GetTodaySteps(5));
    }

    [Fact]
    public void Controller_ReturnsOkForAValidEntry()
    {
        var (service, _, _) = CreateService();
        var controller = new StepsController(service);

        var result = controller.CreateStepEntry(new CreateStepEntryRequest { StepCount = 10, UserId = 1 });

        Assert.IsType<OkResult>(result);
        Assert.Equal(10, service.GetTodaySteps(1));
    }

    [Fact]
    public void Controller_ReturnsBadRequestForAnInvalidEntry()
    {
        var (service, _, _) = CreateService();
        var controller = new StepsController(service);

        var result = controller.CreateStepEntry(new CreateStepEntryRequest { StepCount = 0, UserId = 1 });

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
