using EvolveJourneyLog.Core.Repositories;
using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Models;
using EvolveJourneyLog.Core.Repositories.Pocos;
using EvolveJourneyLog.Core.Services;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests;

[Collection("Database collection"), Trait("Category", "Slow")]
public sealed class GameSaveServiceTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;
    private readonly GameSaveRepository _gameSaveRepository;
    private readonly PrestigeResourceRepository _prestigeResourceRepository;
    private readonly GameSaveService _gameSaveService;
    private readonly PlayerRepository _playerRepository;

    public GameSaveServiceTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
        _gameSaveRepository = new GameSaveRepository(_databaseFactory);
        _prestigeResourceRepository = new PrestigeResourceRepository(_databaseFactory);
        _playerRepository = new PlayerRepository(_databaseFactory);
        _gameSaveService = new GameSaveService(_gameSaveRepository, _prestigeResourceRepository);
    }

    public void Dispose()
    {
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [gamedata].[PrestigeResources];");
        database.Execute("DELETE FROM [gamedata].[GameSave];");
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanHandleUserUploadAndPersistData()
    {
        var playerId = await _playerRepository.SaveAsync("TestPlayer");
        var fileContent = File.ReadAllText(@".\TestInput\evolve-2023-10-10-20-38.txt");

        var result = await _gameSaveService.HandleUserUploadAsync(playerId, fileContent);

        result.Should().Be(SaveResult.Success);

        using var database = _databaseFactory.GetDatabase();
        var retrievedResources = await database.FetchAsync<PrestigeResourcePoco>();
        retrievedResources.Should().HaveCount(1);
        var retrievedResource = retrievedResources[0];
        retrievedResource.Plasmid.Should().Be(27273);
        retrievedResource.AntiPlasmid.Should().Be(3974);
        retrievedResource.Phage.Should().Be(1850);
        retrievedResource.DarkEnergy.Should().Be(4.25);
        retrievedResource.HarmonyCrystal.Should().Be(22);
        retrievedResource.AICore.Should().Be(0);
        retrievedResource.Artifact.Should().Be(0);
        retrievedResource.BloodStone.Should().Be(0);
    }
}
