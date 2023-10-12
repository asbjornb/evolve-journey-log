using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Models;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public sealed class PrestigeResourceRepositoryTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;
    private readonly PlayerRepository _playerRepository;
    private readonly GameSaveRepository _gameSaveRepository;
    private readonly PrestigeResourceRepository _prestigeResourceRepository;

    public PrestigeResourceRepositoryTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
        _playerRepository = new PlayerRepository(_databaseFactory);
        _gameSaveRepository = new GameSaveRepository(_databaseFactory);
        _prestigeResourceRepository = new PrestigeResourceRepository(_databaseFactory);
    }

    public void Dispose()
    {
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [gamedata].[PrestigeResources];");
        database.Execute("DELETE FROM [gamedata].[GameSave];");
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanSavePrestigeResources()
    {
        var playerId = await _playerRepository.SaveAsync("TestPlayer");
        const string rawSaveData = "TestSaveData";

        var saveResult = await _gameSaveRepository.SaveAsync(playerId, rawSaveData);
        saveResult.Should().BeOfType(typeof(SaveSuccess));

        var gameSaveId = (saveResult as SaveSuccess)!.SaveId;
        var prestigeSaveResult = await _prestigeResourceRepository.SaveAsync(gameSaveId, 1, 2, 3, 4, 5, 6, 7, 8);
        prestigeSaveResult.Should().Be(SaveResult.Success);

        using var database = _databaseFactory.GetDatabase();
        var retrievedAICore = await database.ExecuteScalarAsync<int>("SELECT TOP 1 AICore FROM [gamedata].[PrestigeResources] WHERE SaveId = @0;", gameSaveId);

        retrievedAICore.Should().Be(1);
    }
}
