using System.Data.SqlClient;
using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
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

        await _gameSaveRepository.SaveAsync(playerId, rawSaveData);

        using var database = _databaseFactory.GetDatabase();
        var retrievedRawSaveData = await database.ExecuteScalarAsync<string>("SELECT TOP 1 RawSaveData FROM [gamedata].[GameSave] WHERE PlayerId = @0;", playerId);

        retrievedRawSaveData.Should().Be(rawSaveData);
    }

    [Fact]
    public async Task CannotInsertDuplicateSaveData()
    {
        var playerId = await _playerRepository.SaveAsync("TestPlayer");
        const string rawSaveData = "DuplicateSaveData";

        await _gameSaveRepository.SaveAsync(playerId, rawSaveData);

        using var database = _databaseFactory.GetDatabase();
        await Assert.ThrowsAsync<SqlException>(() => _gameSaveRepository.SaveAsync(playerId, rawSaveData));
    }
}
