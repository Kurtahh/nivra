using backend.Interfaces;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>((DbContextOptionsBuilder options) =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStepsRepo, StepsRepo>();
builder.Services.AddScoped<IStepsService, StepsService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHealthTipRepo, HealthTipRepo>();
builder.Services.AddScoped<IHealthTipService, HealthTipService>();
builder.Services.AddScoped<ILeaderboardRepo, LeaderboardRepo>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IChatRepo, ChatRepo>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped(typeof(IShopRepository<>), typeof(ShopRepository<>));
builder.Services.AddScoped<ISkinService, SkinService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

var dbConnectCheck = app.Services.CreateScope();
var scopedDbContext = dbConnectCheck.ServiceProvider.GetRequiredService<AppDbContext>();
Console.WriteLine(scopedDbContext.Database.CanConnect() ? "Connected!" : "Could not connect.");
dbConnectCheck.Dispose();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();