using backend.Controllers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace nivra.api.Tests.Shop;

public class OrderTests
{
    private static (OrderController Controller, ShopRepository<Order> Repository)
        CreateController()
    {
        var context = TestDb.Create();
        var repository = new ShopRepository<Order>(context);
        var service = new OrderService(repository);
        return (new OrderController(service), repository);
    }

    private static Order NewOrder(int id = 0, string name = "Shoes", int price = 50)
    {
        return new Order { Id = id, Name = name, Price = price };
    }

    [Fact]
    public void GetOrders_ReturnsAllOrders()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewOrder(name: "First"));
        repository.Add(NewOrder(name: "Second"));

        var result = controller.GetOrders();

        var ok = Assert.IsType<OkObjectResult>(result);
        var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(ok.Value);
        Assert.Equal(new[] { "First", "Second" }, orders.Select(order => order.Name));
    }

    [Fact]
    public void GetOrderById_ReturnsOrder_WhenItExists()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewOrder(id: 7, name: "Hat", price: 20));

        var result = controller.GetOrderById(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        var order = Assert.IsType<Order>(ok.Value);
        Assert.Equal("Hat", order.Name);
        Assert.Equal(20, order.Price);
    }

    [Fact]
    public void GetOrderById_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.GetOrderById(404);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public void CreateOrder_ReturnsCreatedAndPersistsTheOrder()
    {
        var (controller, repository) = CreateController();

        var result = controller.CreateOrder(NewOrder(id: 99, name: "New", price: 10));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var order = Assert.IsType<Order>(created.Value);
        Assert.Equal("New", order.Name);
        Assert.Equal(10, order.Price);
        Assert.Contains(repository.GetAllItems(), item => item.Name == "New");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateOrder_ReturnsBadRequest_WhenNameIsEmpty(string name)
    {
        var (controller, _) = CreateController();

        var result = controller.CreateOrder(NewOrder(name: name));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateOrder_ReturnsBadRequest_WhenPriceIsNegative()
    {
        var (controller, _) = CreateController();

        var result = controller.CreateOrder(NewOrder(price: -1));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateOrder_ReturnsOkAndSavesTheChanges()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewOrder(id: 3, name: "Old", price: 5));

        var result = controller.UpdateOrder(3, NewOrder(name: "Updated", price: 50));

        var ok = Assert.IsType<OkObjectResult>(result);
        var order = Assert.IsType<Order>(ok.Value);
        Assert.Equal("Updated", order.Name);
        Assert.Equal(50, order.Price);
        Assert.Equal("Updated", repository.GetByIdNullable(3)!.Name);
    }

    [Fact]
    public void UpdateOrder_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateOrder(404, NewOrder());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateOrder_ReturnsBadRequest_WhenTheNewValuesAreInvalid()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateOrder(1, NewOrder(name: " "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DeleteOrder_ReturnsNoContentAndRemovesTheOrder()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewOrder(id: 8));

        var result = controller.DeleteOrder(8);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(repository.GetByIdNullable(8));
    }

    [Fact]
    public void DeleteOrder_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.DeleteOrder(404);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
