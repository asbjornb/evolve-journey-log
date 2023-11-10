using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Pocos;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests.IntegrationTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public class PlayerRegistrationTests : IAsyncLifetime
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly WebApplication _app;
    private readonly DatabaseFactory _databaseFactory;
    private const string SitePath = "WebContent/index.html";
    private const string ApiUrl = "https://localhost:7274";

    public PlayerRegistrationTests(DatabaseSetup databaseSetup)
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
    }

    public async Task InitializeAsync()
    {
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions());
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions() { IgnoreHTTPSErrors = true });
        _ = Task.Run(() => _app.Run(ApiUrl));
        await Task.Delay(3000);
    }

    public async Task DisposeAsync()
    {
        if(_context is not null)
        {
            await _context.CloseAsync();
        }
        if(_browser is not null)
        {
            await _browser.CloseAsync();
        }
        await _app.StopAsync();
    }

    [Fact]
    public async Task TestEvolveJourneyLogFunctionality()
    {
        if(_browser is null || _context is null)
        {
            throw new InvalidOperationException("The browser instance is null. Error in setup.");
        }

        const string TestPlayerName = "TestPlayerName";

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

        var requestUrls = new List<string>();

        page.Request += (_, request) => requestUrls.Add(request.Url);

        await Task.Delay(1000); // Give the js script time to load

        await page.ClickAsync("button:has-text('Register Player')");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Task.Delay(1000);

        // Check that dialog was triggered
        isDialogTriggered.Should().BeTrue();

        // Check that request was sent to the correct url
        requestUrls.Should().HaveCount(1);
        requestUrls[0].Should().Be(ApiUrl + "/api/player/register");

        // Check that playerId is set in local storage
        var playerId = await page.EvaluateAsync<string>("localStorage.getItem('playerId')");
        playerId.Should().NotBeNull();

        // Check that player is registered in the database
        using var database = _databaseFactory.GetDatabase();
        var players = await database.FetchAsync<PlayerPoco>();
        players.Should().HaveCount(1);
        var databasePlayerName = players[0].PlayerName;
        databasePlayerName.Should().Be(TestPlayerName);
    }

    private static string GetSiteUrl()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var absolutePath = Path.GetFullPath(Path.Combine(baseDirectory, SitePath));
        return "file://" + absolutePath;
    }
}
