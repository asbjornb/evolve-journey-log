using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Services;

var builder = WebApplication.CreateBuilder(args);

//Configurations
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing connection string in configuration.");
builder.Services.AddSingleton<IDatabaseFactory>(new DatabaseFactory(connectionString));

// Add services to the container.
builder.Services.AddTransient<PlayerRepository>();
builder.Services.AddTransient<GameSaveRepository>();
builder.Services.AddTransient<PrestigeResourceRepository>();
builder.Services.AddTransient<PlayerService>();
builder.Services.AddTransient<GameSaveService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(); //For a short while use swagger in production
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
