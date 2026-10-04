using backend.Models;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;
using backend.Enums;

namespace nivra.api.Tests;

public class UserRepoTests
{
    [Fact]
    public async Task AddSavesNewUser()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);

        var user = new User
        {
            Username = "vincent",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };

        repository.Add(user);

        var savedUser = await context.Users.FindAsync(3);
        Assert.NotNull(savedUser);
        Assert.Equal("vincent", savedUser.Username);
    }


    [Fact]
    public async Task AddStopsIfUserAlreadyExists()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);

        var user = new User
        {
            Username = "vincent",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };
        context.Add(user);

        repository.Add(user);

        var savedUser = await context.Users.FindAsync(3);
        Assert.NotNull(savedUser);
        Assert.Equal(1, context.Users.Count());
    }
    
    [Fact]
    public async Task ReturnEnumWithSuccessValue()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);
        Status success = Status.Success;
        

        var user = new User
        {
            Username = "vincent",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };



        var value = repository.Add(user);

        var savedUser = await context.Users.FindAsync(3);
        Assert.NotNull(savedUser);
        Assert.Equal(Status.Success, value);
    }
    
    
}
