using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Pocos;

namespace EvolveJourneyLog.Core.Repositories;

public class GameSaveRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public GameSaveRepository(IDatabaseFactory dbFactory)
    {
        _databaseFactory = dbFactory;
    }

    public async Task SaveAsync(Guid playerId, string rawSaveData)
    {
        var gameSave = new GameSavePoco(playerId, rawSaveData);
        using var database = _databaseFactory.GetDatabase();
        await database.InsertAsync(gameSave);
    }
}
