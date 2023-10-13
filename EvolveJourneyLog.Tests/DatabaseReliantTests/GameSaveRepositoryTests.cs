using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Models;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public sealed class GameSaveRepositoryTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;
    private readonly PlayerRepository _playerRepository;
    private readonly GameSaveRepository _gameSaveRepository;

    public GameSaveRepositoryTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
        _playerRepository = new PlayerRepository(_databaseFactory);
        _gameSaveRepository = new GameSaveRepository(_databaseFactory);
    }

    public void Dispose()
    {
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [gamedata].[GameSave];");
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanSaveGameAndRetrieveData()
    {
        var playerId = await _playerRepository.SaveAsync("TestPlayer");
        const string rawSaveData = "TestSaveData";

        var result = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);
        result.Should().BeOfType(typeof(SaveSuccess));

        using var database = _databaseFactory.GetDatabase();
        var retrievedRawSaveData = await database.ExecuteScalarAsync<string>("SELECT TOP 1 RawSaveData FROM [gamedata].[GameSave] WHERE PlayerId = @0;", playerId);

        retrievedRawSaveData.Should().Be(rawSaveData);
    }

    [Fact]
    public async Task CannotInsertDuplicateSaveData()
    {
        var playerId = await _playerRepository.SaveAsync("TestPlayer");
        const string rawSaveData = "DuplicateSaveData";

        var firstSaveResult = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);
        firstSaveResult.Should().BeOfType(typeof(SaveSuccess)); // Check if the first save was successful

        var secondSaveResult = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);
        secondSaveResult.Should().BeEquivalentTo(new SaveFailure(SaveResult.DuplicateSave)); // Check if the second save is detected as duplicate
    }

    [Fact]
    public async Task SavingForNonExistingPlayerReturnsPlayerNotFoundError()
    {
        var nonExistingPlayerId = Guid.NewGuid();
        const string rawSaveData = "TestData";

        var saveResult = await _gameSaveRepository.SaveAsync(nonExistingPlayerId, rawSaveData);

        saveResult.Should().BeEquivalentTo(new SaveFailure(SaveResult.PlayerNotFound));
    }
}
