using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Pocos;

namespace EvolveJourneyLog.Core.Repositories;

public class PlayerRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public PlayerRepository(IDatabaseFactory dbFactory)
    {
        _databaseFactory = dbFactory;
    }

    public async Task<Guid> SaveAsync(string playerName)
    {
        var player = new PlayerPoco(playerName);

        using var database = _databaseFactory.GetDatabase();
        await database.InsertAsync(player);

        return player.PlayerId;
    }
}
