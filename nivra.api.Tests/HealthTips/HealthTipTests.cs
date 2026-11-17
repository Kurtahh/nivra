using backend.Controllers;
using backend.Enums;
using backend.Exceptions;
using backend.Extensions;
using backend.Interfaces;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace nivra.api.Tests.HealthTips;

public class HealthTipTests
{
    [Fact]
    public void Repository_AddsAndReturnsTips()
    {
        var context = TestDb.Create();
        var repository = new HealthTipRepo(context);
        var tip = new HealthTip(1, "Drink water", TipCategory.Hydration);

        repository.Add(tip);

        var saved = Assert.Single(repository.GetAll());
        Assert.Equal("Drink water", saved.Content);
        Assert.Equal(TipCategory.Hydration, saved.Category);
    }

    [Fact]
    public void Repository_PersistsCategoriesAsText()
    {
        var context = TestDb.Create();
        var repository = new HealthTipRepo(context);
        repository.Add(new HealthTip(1, "Walk", TipCategory.Exercise));

        var saved = context.HealthTips.AsNoTracking().Single();

        Assert.Equal(TipCategory.Exercise, saved.Category);
    }

    [Fact]
    public void Service_ReturnsTheOnlyAvailableTip()
    {
        var context = TestDb.Create();
        var repository = new HealthTipRepo(context);
        repository.Add(new HealthTip(1, "Sleep well", TipCategory.Sleep));
        var service = new HealthTipService(repository);

        var tip = service.GetRandomTip();

        Assert.Equal("Sleep well", tip.Content);
    }

    [Fact]
    public void Service_ThrowsWhenThereAreNoTips()
    {
        var service = new HealthTipService(new HealthTipRepo(TestDb.Create()));

        Assert.Throws<ArgumentException>(() => service.GetRandomTip());
    }

    [Fact]
    public void Controller_ReturnsOkWithATip()
    {
        var context = TestDb.Create();
        var repository = new HealthTipRepo(context);
        repository.Add(new HealthTip(1, "Stretch", TipCategory.Exercise));
        var controller = new HealthTipController(new HealthTipService(repository));

        var result = controller.GetRandomTip();

        var ok = Assert.IsType<OkObjectResult>(result);
        var tip = Assert.IsType<HealthTip>(ok.Value);
        Assert.Equal("Stretch", tip.Content);
    }

    [Fact]
    public void Controller_ReturnsNotFoundWhenNoTipsExist()
    {
        var controller = new HealthTipController(
            new HealthTipService(new HealthTipRepo(TestDb.Create())));

        var result = controller.GetRandomTip();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void Controller_ReturnsInternalServerErrorForAnInvalidCategory()
    {
        var service = new Mock<IHealthTipService>();
        service
            .Setup(healthTipService => healthTipService.GetRandomTip())
            .Throws(new InvalidTipCategoryException("unknown", Enum.GetNames<TipCategory>()));
        var controller = new HealthTipController(service.Object);

        var result = controller.GetRandomTip();

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Theory]
    [InlineData("nutrition", TipCategory.Nutrition)]
    [InlineData("EXERCISE", TipCategory.Exercise)]
    [InlineData("Sleep", TipCategory.Sleep)]
    public void StringExtensions_ParseCategoriesIgnoringCase(string text, TipCategory expected)
    {
        Assert.Equal(expected, text.ToTipCategory());
    }

    [Fact]
    public void StringExtensions_ThrowForAnUnknownCategory()
    {
        var exception = Assert.Throws<InvalidTipCategoryException>(() => "junk".ToTipCategory());

        Assert.Equal("junk", exception.InvalidValue);
        Assert.Contains("junk", exception.Message);
        Assert.Contains("Nutrition", exception.Message);
    }
}
