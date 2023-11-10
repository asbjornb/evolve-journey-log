using System.Net;
using System.Text;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Pocos;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Moq;
using PetaPoco;

namespace EvolveJourneyLog.Tests.ApiTests;

public class ApiConnectionTests3 : IAsyncLifetime
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly WebApplication _app;
    private const string ApiUrl = "https://localhost:7274";
    private const string SitePath = "WebContent/index.html";

    public ApiConnectionTests3()
    {
        var mockDatabase = new Mock<IDatabase>();
        var returnId = Guid.NewGuid();
        mockDatabase.Setup(x => x.InsertAsync(It.IsAny<object>()))
            .Callback((object obj) =>
            {
                if (obj is PlayerPoco player)
                {
                    player.PlayerId = returnId;
                }
            })
            .Returns(Task.FromResult((object)returnId));
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
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(); //Consider new BrowserTypeLaunchOptions() { Headless = false } if doing manual debugging
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions() { IgnoreHTTPSErrors = true });
        _ = Task.Run(() => _app.Run(ApiUrl));
        await Task.Delay(3000);
    }

    public async Task DisposeAsync()
    {
        await _app.StopAsync();
    }

    [Fact]
    public async Task TestApiConnectionWithLocalStorage()
    {
        if (_browser is null || _context is null)
        {
            throw new InvalidOperationException("The browser instance is null. Error in setup.");
        }

        const string TestPlayerName = "TestPlayerName";

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        using var client = new HttpClient(handler);

        // Register player through API
        var registerPlayerRequest = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}/api/Player/register")
        {
            Content = new StringContent($"{{\"playerName\":\"{TestPlayerName}\"}}", Encoding.UTF8, "application/json")
        };
        var registerPlayerResponse = await client.SendAsync(registerPlayerRequest);
        registerPlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Now test with Playwright
        var page = await _context.NewPageAsync();
        await page.GotoAsync(GetSiteUrl());

        var isDialogTriggered = false;
        page.Dialog += async (_, dialog) =>
        {
            if (dialog.Type == DialogType.Prompt)
            {
                isDialogTriggered = true;
                await dialog.AcceptAsync(TestPlayerName);
            }
        };

        await Task.Delay(1000); // Give the js script time to load

        await page.ClickAsync("button:has-text('Register Player')");

        isDialogTriggered.Should().BeTrue();

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Task.Delay(1000);

        var playerId = await page.EvaluateAsync<string>("localStorage.getItem('playerId')");
        playerId.Should().NotBeNull();
        var guid = Guid.Parse(playerId);
        guid.Should().NotBeEmpty();
    }

    private static string GetSiteUrl()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var absolutePath = Path.GetFullPath(Path.Combine(baseDirectory, SitePath));
        return "file://" + absolutePath;
    }
}
