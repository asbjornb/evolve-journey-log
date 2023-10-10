using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public sealed class PlayerRepositoryTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;
    private readonly PlayerRepository _playerRepository;

    public PlayerRepositoryTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
        _playerRepository = new PlayerRepository(_databaseFactory);
    }

    public void Dispose()
    {
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanSavePlayerAndGetPlayerId()
    {
        var playerId = await _playerRepository.SaveAsync("TestName");

        using var database = _databaseFactory.GetDatabase();
        var retrievedPlayerId = await database.ExecuteScalarAsync<Guid>("SELECT TOP 1 PlayerId FROM [gamedata].[Player] WHERE PlayerName = 'TestName';");

        playerId.Should().Be(retrievedPlayerId);
    }
}
