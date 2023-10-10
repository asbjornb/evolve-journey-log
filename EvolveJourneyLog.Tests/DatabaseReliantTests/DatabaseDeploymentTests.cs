using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public sealed class DatabaseDeploymentTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;

    public DatabaseDeploymentTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
    }

    public void Dispose()
    {
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanWriteAndReadFromDatabase()
    {
        using var database = _databaseFactory.GetDatabase();

        // Insert data
        database.Execute("INSERT INTO [gamedata].[Player] (PlayerName) VALUES ('SomeName');");

        // Read data
        var playerName = await database.ExecuteScalarAsync<string>("SELECT TOP 1 PlayerName FROM [gamedata].[Player];");

        playerName.Should().Be("SomeName");
    }
}
