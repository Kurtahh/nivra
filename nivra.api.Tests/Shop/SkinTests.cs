using backend.Controllers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace nivra.api.Tests.Shop;

public class SkinTests
{
    private static (SkinController Controller, ShopRepository<Skin> Repository)
        CreateController()
    {
        var context = TestDb.Create();
        var repository = new ShopRepository<Skin>(context);
        var service = new SkinService(repository);
        return (new SkinController(service), repository);
    }

    private static Skin NewSkin(int id = 0, string name = "Default", int price = 10)
    {
        return new Skin { Id = id, Name = name, Price = price };
    }

    [Fact]
    public void GetSkins_ReturnsAllSkins()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewSkin(name: "First"));
        repository.Add(NewSkin(name: "Second"));

        var result = controller.GetSkins();

        var ok = Assert.IsType<OkObjectResult>(result);
        var skins = Assert.IsAssignableFrom<IEnumerable<Skin>>(ok.Value);
        Assert.Equal(new[] { "First", "Second" }, skins.Select(skin => skin.Name));
    }

    [Fact]
    public void GetSkinById_ReturnsSkin_WhenItExists()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewSkin(id: 7, name: "Neon", price: 20));

        var result = controller.GetSkinById(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        var skin = Assert.IsType<Skin>(ok.Value);
        Assert.Equal("Neon", skin.Name);
        Assert.Equal(20, skin.Price);
    }

    [Fact]
    public void GetSkinById_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.GetSkinById(404);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public void CreateSkin_ReturnsCreatedAndPersistsTheSkin()
    {
        var (controller, repository) = CreateController();

        var result = controller.CreateSkin(NewSkin(id: 99, name: "New", price: 10));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var skin = Assert.IsType<Skin>(created.Value);
        Assert.Equal("New", skin.Name);
        Assert.Equal(10, skin.Price);
        Assert.Contains(repository.GetAllItems(), item => item.Name == "New");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateSkin_ReturnsBadRequest_WhenNameIsEmpty(string name)
    {
        var (controller, _) = CreateController();

        var result = controller.CreateSkin(NewSkin(name: name));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateSkin_ReturnsBadRequest_WhenPriceIsNegative()
    {
        var (controller, _) = CreateController();

        var result = controller.CreateSkin(NewSkin(price: -1));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateSkin_ReturnsOkAndSavesTheChanges()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewSkin(id: 3, name: "Old", price: 5));

        var result = controller.UpdateSkin(3, NewSkin(name: "Updated", price: 50));

        var ok = Assert.IsType<OkObjectResult>(result);
        var skin = Assert.IsType<Skin>(ok.Value);
        Assert.Equal("Updated", skin.Name);
        Assert.Equal(50, skin.Price);
        Assert.Equal("Updated", repository.GetById(3)!.Name);
    }

    [Fact]
    public void UpdateSkin_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateSkin(404, NewSkin());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateSkin_ReturnsBadRequest_WhenTheNewValuesAreInvalid()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateSkin(1, NewSkin(name: " "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DeleteSkin_ReturnsNoContentAndRemovesTheSkin()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewSkin(id: 8));

        var result = controller.DeleteSkin(8);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(repository.GetById(8));
    }

    [Fact]
    public void DeleteSkin_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.DeleteSkin(404);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
