using System.Net;
using System.Text;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PetaPoco;

namespace EvolveJourneyLog.Tests.ApiTests;

public class ApiConnectionTests2 : IAsyncLifetime
{
    private readonly WebApplication _app;
    private const string ApiUrl = "https://localhost:7274";

    public ApiConnectionTests2()
    {
        var mockDatabase = new Mock<IDatabase>();
        mockDatabase.Setup(x => x.SaveAsync(It.IsAny<object>())).Returns(Task.FromResult(1));
        var mockDatabaseFactory = new Mock<IDatabaseFactory>();
        mockDatabaseFactory.Setup(x => x.GetDatabase()).Returns(mockDatabase.Object);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "SomeNonsense",
                ["CorsOrigins:0"] = "file://",
                ["AllowedHosts"] = "*"
            })
            .Build();
        var builder = Api.Program.SetupBuilder(config);
        builder.Services.AddSingleton(mockDatabaseFactory.Object);
        _app = Api.Program.BuildAndConfigureApp(builder);
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
        registerPlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
