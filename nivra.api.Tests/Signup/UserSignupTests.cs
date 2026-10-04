using backend.Enums;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace nivra.api.Tests.Signup;

public class UserSignupTests
{
    [Fact]
     public async Task SignUpReturnsSuccessIfUserSignsUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);
        var service = new UserService(repository);
        
        var user = new User
        {
            Username = "vincent",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };
        
        
       var value = service.SignUpUser(user);
        
       Assert.Equal(Status.Success, value);
    }
     
    [Fact]
    public async Task SignUpReturnsFailureIfUserSignsUpWithTheSameUsername()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);
        var service = new UserService(repository);
        
        var user = new User
        {
            Username = "vincent",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };
        
        
        var value = service.SignUpUser(user);
         value = service.SignUpUser(user);
        
        Assert.Equal(Status.Failure, value);
    }
    
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
        repository.Add(user);

        repository.Add(user);

        var savedUser = await context.Users.FindAsync(3);
        Assert.NotNull(savedUser);
        Assert.Equal(1, context.Users.Count());
    }
    
    [Fact]
    public async Task AddReturnsEnumWithSuccessValueIfSignInSuccessful()
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



        var value = repository.Add(user);

        var savedUser = await context.Users.FindAsync(3);
        Assert.NotNull(savedUser);
        Assert.Equal(Status.Success, value);
    }
    
    
    [Fact]
    public async Task AddReturnsEnumWithFailureValue()
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

        var value = repository.Add(user);

        Assert.Equal(Status.Success, value);
        value = repository.Add(user);
        Assert.Equal(Status.Failure, value);

    }
    
    [Fact]
    public async Task AddDoesntLetSaveEmptyUsername()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);

        var user = new User
        {
            Username = "",
            CreatedAt = DateTime.Now,
            PasswordHash = "adfaf",
            Id = 3
        };

        var value = repository.Add(user);

        Assert.Equal(Status.Failure, value);

    }
    
    [Fact]
    public async Task AddDoesntLetSaveEmptyPassword()
    {
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new UserRepo(context);

        var user = new User
        {
            Username = "Name",
            CreatedAt = DateTime.Now,
            PasswordHash = "",
            Id = 3
        };

        var value = repository.Add(user);

        Assert.Equal(Status.Failure, value);

    }
    
    
}