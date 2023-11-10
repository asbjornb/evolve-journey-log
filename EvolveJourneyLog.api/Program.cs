using EvolveJourneyLog.Api.Controllers;
using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Services;

namespace EvolveJourneyLog.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var config = GenerateConfiguration(args);
        var builder = SetupBuilder(config);
        var app = BuildAndConfigureApp(builder);
        app.Run();
    }

    public static IConfiguration GenerateConfiguration(string[] args)
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return configBuilder.Build();
    }

    public static WebApplicationBuilder SetupBuilder(IConfiguration config)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

        //Configurations
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing connection string in configuration.");

        // Add services to the container
        builder.Services.AddSingleton<IDatabaseFactory>(new DatabaseFactory(connectionString));
        builder.Services.AddTransient<PlayerRepository>();
        builder.Services.AddTransient<GameSaveRepository>();
        builder.Services.AddTransient<PrestigeResourceRepository>();
        builder.Services.AddTransient<PlayerService>();
        builder.Services.AddTransient<GameSaveService>();

        // This was neccesary to be able to run from the test-project. Otherwise reflection won't pick up controllers since they are in a different assembly.
        builder.Services.AddControllers().AddApplicationPart(typeof(PlayerController).Assembly);
        builder.Services.AddCors();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplication BuildAndConfigureApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger(); //For a short while use swagger in production
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        //var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? throw new InvalidOperationException("Missing cors origins in configuration.");
        //app.UseCors(builder =>
        //    builder.WithOrigins(corsOrigins)
        //           .AllowAnyMethod()
        //           .AllowAnyHeader()
        //);
        //Temporarily allow all origins
        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.MapControllers();
        app.UseAuthorization();

        return app;
    }
}
