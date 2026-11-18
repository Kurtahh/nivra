using backend.Controllers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace nivra.api.Tests.Shop;

public class AccountTests
{
    private static (AccountController Controller, ShopRepository<Account> Repository)
        CreateController()
    {
        var context = TestDb.Create();
        var repository = new ShopRepository<Account>(context);
        var service = new AccountService(repository);
        return (new AccountController(service), repository);
    }

    private static Account NewAccount(int id = 0, string name = "Savings", int balance = 100)
    {
        return new Account { Id = id, Name = name, Balance = balance };
    }

    [Fact]
    public void GetAccounts_ReturnsAllAccounts()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewAccount(name: "First"));
        repository.Add(NewAccount(name: "Second"));

        var result = controller.GetAccounts();

        var ok = Assert.IsType<OkObjectResult>(result);
        var accounts = Assert.IsAssignableFrom<IEnumerable<Account>>(ok.Value);
        Assert.Equal(new[] { "First", "Second" }, accounts.Select(account => account.Name));
    }

    [Fact]
    public void GetAccountById_ReturnsAccount_WhenItExists()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewAccount(id: 7, name: "Wallet", balance: 42));

        var result = controller.GetAccountById(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        var account = Assert.IsType<Account>(ok.Value);
        Assert.Equal("Wallet", account.Name);
        Assert.Equal(42, account.Balance);
    }

    [Fact]
    public void GetAccountById_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.GetAccountById(404);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public void CreateAccount_ReturnsCreatedAndPersistsTheAccount()
    {
        var (controller, repository) = CreateController();

        var result = controller.CreateAccount(NewAccount(id: 99, name: "New", balance: 10));

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var account = Assert.IsType<Account>(created.Value);
        Assert.Equal("New", account.Name);
        Assert.Equal(10, account.Balance);
        Assert.Contains(repository.GetAllItems(), item => item.Name == "New");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateAccount_ReturnsBadRequest_WhenNameIsEmpty(string name)
    {
        var (controller, _) = CreateController();

        var result = controller.CreateAccount(NewAccount(name: name));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateAccount_ReturnsBadRequest_WhenBalanceIsNegative()
    {
        var (controller, _) = CreateController();

        var result = controller.CreateAccount(NewAccount(balance: -1));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateAccount_ReturnsOkAndSavesTheChanges()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewAccount(id: 3, name: "Old", balance: 5));

        var result = controller.UpdateAccount(3, NewAccount(name: "Updated", balance: 50));

        var ok = Assert.IsType<OkObjectResult>(result);
        var account = Assert.IsType<Account>(ok.Value);
        Assert.Equal("Updated", account.Name);
        Assert.Equal(50, account.Balance);
        Assert.Equal("Updated", repository.GetById(3)!.Name);
    }

    [Fact]
    public void UpdateAccount_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateAccount(404, NewAccount());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void UpdateAccount_ReturnsBadRequest_WhenTheNewValuesAreInvalid()
    {
        var (controller, _) = CreateController();

        var result = controller.UpdateAccount(1, NewAccount(name: " "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DeleteAccount_ReturnsNoContentAndRemovesTheAccount()
    {
        var (controller, repository) = CreateController();
        repository.Add(NewAccount(id: 8));

        var result = controller.DeleteAccount(8);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(repository.GetById(8));
    }

    [Fact]
    public void DeleteAccount_ReturnsNotFound_WhenItDoesNotExist()
    {
        var (controller, _) = CreateController();

        var result = controller.DeleteAccount(404);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
