using backend.Controllers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace nivra.api.Tests.Signup;

public class AuthenticationTests
{
    private static (UserService Service, UserRepo Repository, AppDbContext Context) CreateService()
    {
        var context = TestDb.Create();
        var repository = new UserRepo(context);
        return (new UserService(repository), repository, context);
    }

    private static User NewUser(string username = "vincent", string password = "secret")
    {
        return new User { Username = username, PasswordHash = password };
    }

    [Fact]
    public void SignUp_HashesThePassword()
    {
        var (service, repository, _) = CreateService();

        var status = service.SignUpUser(NewUser());

        Assert.Equal(backend.Enums.Status.Success, status);
        Assert.NotEqual("secret", repository.GetUserNullable("vincent")!.PasswordHash);
    }

    [Fact]
    public void CheckPassword_ReturnsTrueForTheCorrectPassword()
    {
        var (service, repository, _) = CreateService();
        service.SignUpUser(NewUser(password: "correct"));
        var databaseUser = repository.GetUserNullable("vincent")!;
        var inputUser = NewUser(password: "correct");

        Assert.True(service.CheckPassword(inputUser, databaseUser));
    }

    [Fact]
    public void CheckPassword_ReturnsFalseForAWrongPassword()
    {
        var (service, repository, _) = CreateService();
        service.SignUpUser(NewUser(password: "correct"));
        var databaseUser = repository.GetUserNullable("vincent")!;
        var inputUser = NewUser(password: "wrong");

        Assert.False(service.CheckPassword(inputUser, databaseUser));
    }

    [Fact]
    public void CheckIfUsernameExists_ReturnsTheUserOrNull()
    {
        var (service, _, _) = CreateService();
        service.SignUpUser(NewUser("alice"));

        Assert.NotNull(service.CheckIfUsernameExists("alice"));
        Assert.Null(service.CheckIfUsernameExists("nobody"));
    }

    [Fact]
    public void GetUserByName_ReturnsTheUserOrNull()
    {
        var (service, _, _) = CreateService();
        service.SignUpUser(NewUser("alice"));

        Assert.Equal("alice", service.GetUserByName("alice")!.Username);
        Assert.Null(service.GetUserByName("nobody"));
    }

    [Fact]
    public void Repository_GetUserReturnsNullForAnUnknownUser()
    {
        var (_, repository, _) = CreateService();

        Assert.Null(repository.GetUserNullable("nobody"));
    }

    private static UserController CreateController(UserService service, StepsService stepsService)
    {
        return new UserController(service, stepsService);
    }

    [Fact]
    public void Authenticate_ReturnsNotFoundWhenTheUsernameDoesNotExist()
    {
        var (service, _, _) = CreateService();
        var stepsService = new StepsService(new StepsRepo(TestDb.Create()));
        var controller = CreateController(service, stepsService);

        var result = controller.AuthenticateUser(NewUser("nobody", "secret"));

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public void Authenticate_ReturnsNotFoundWhenThePasswordIsWrong()
    {
        var (service, _, _) = CreateService();
        service.SignUpUser(NewUser("alice", "correct"));
        var stepsService = new StepsService(new StepsRepo(TestDb.Create()));
        var controller = CreateController(service, stepsService);

        var result = controller.AuthenticateUser(NewUser("alice", "wrong"));

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Fact]
    public void Authenticate_ReturnsOkWithTheUserIdWhenCredentialsAreCorrect()
    {
        var (service, repository, _) = CreateService();
        service.SignUpUser(NewUser("alice", "correct"));
        var databaseUser = repository.GetUserNullable("alice")!;
        var stepsService = new StepsService(new StepsRepo(TestDb.Create()));
        var controller = CreateController(service, stepsService);

        var result = controller.AuthenticateUser(NewUser("alice", "correct"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<string[]>(ok.Value);
        Assert.Equal(new[] { "Login successful!", "true", databaseUser.Id.ToString() }, payload);
    }

    [Fact]
    public void CreateUserOnSignUp_ReturnsOkForANewUser()
    {
        var (service, _, _) = CreateService();
        var stepsService = new StepsService(new StepsRepo(TestDb.Create()));
        var controller = CreateController(service, stepsService);

        var result = controller.CreateUserOnSignUp(NewUser("alice", "correct"));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void CreateUserOnSignUp_ReturnsConflictForADuplicateUser()
    {
        var (service, _, _) = CreateService();
        var stepsService = new StepsService(new StepsRepo(TestDb.Create()));
        var controller = CreateController(service, stepsService);
        var user = NewUser("alice", "correct");

        controller.CreateUserOnSignUp(user);
        var result = controller.CreateUserOnSignUp(user);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflict.StatusCode);
    }

    [Fact]
    public void GetTodaySteps_ReturnsTheStepsLoggedForTheUser()
    {
        var (service, _, _) = CreateService();
        var stepsRepository = new StepsRepo(TestDb.Create());
        var stepsService = new StepsService(stepsRepository);
        var controller = CreateController(service, stepsService);
        stepsService.LogSteps(555, 12);

        Assert.Equal(555, controller.GetTodaySteps(12));
    }
}
