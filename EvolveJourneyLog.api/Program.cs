using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Services;

namespace EvolveJourneyLog.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var config = GenerateConfiguration(args);
        var app = BuildApp(config);
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

    private static WebApplication BuildApp(IConfiguration config)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddConfiguration(config);

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
        builder.Services.AddCors();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger(); //For a short while use swagger in production
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseCors(builder =>
            builder.WithOrigins("https://asbjornb.github.io")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
        );

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
