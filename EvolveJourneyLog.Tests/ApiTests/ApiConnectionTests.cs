using System.Net;
using System.Text;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Pocos;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EvolveJourneyLog.Tests.ApiTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public class ApiConnectionTests : IAsyncLifetime
{
    private readonly WebApplication _app;
    private readonly DatabaseFactory _databaseFactory;
    private const string ApiUrl = "https://localhost:7274";
    private readonly List<string> _fallbacks = new();

    public ApiConnectionTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = databaseSetup.ConnectionString,
                ["CorsOrigins:0"] = "file://",
                ["AllowedHosts"] = "*"
            })
            .Build();
        var builder = Api.Program.SetupBuilder(config);
        _app = Api.Program.BuildAndConfigureApp(builder);
        _app.MapFallback(context =>
        {
            var message = $"No endpoint matched for {context.Request.Method} {context.Request.Path}";
            _fallbacks.Add(message);
            return context.Response.WriteAsync($"No endpoint matched for {context.Request.Method} {context.Request.Path}");
        });
    }

    public async Task InitializeAsync()
    {
        _ = Task.Run(() => _app.Run(ApiUrl));
        await Task.Delay(3000);
    }

    public async Task DisposeAsync()
    {
        await _app.StopAsync();
    }

    [Fact]
    public async Task TestApiConnection()
    {
        const string TestPlayerName = "TestPlayerName";

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        using var client = new HttpClient(handler);

        // Register player
        var registerPlayerRequest = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}/api/Player/register")
        {
            Content = new StringContent($"{{\"playerName\":\"{TestPlayerName}\"}}", Encoding.UTF8, "application/json")
        };
        var registerPlayerResponse = await client.SendAsync(registerPlayerRequest);
        _fallbacks.Should().BeEmpty();
        registerPlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check that player is registered in the database
        using var database = _databaseFactory.GetDatabase();
        var players = await database.FetchAsync<PlayerPoco>();
        players.Should().HaveCount(1);
        var databasePlayerName = players[0].PlayerName;
        databasePlayerName.Should().Be(TestPlayerName);
    }
}
