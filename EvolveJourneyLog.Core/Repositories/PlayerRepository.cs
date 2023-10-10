using EvolveJourneyLog.Core.Repositories.Pocos;

namespace EvolveJourneyLog.Core.Repositories;

public class PlayerRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public PlayerRepository(IDatabaseFactory dbFactory)
    {
        _databaseFactory = dbFactory;
    }

    public async Task SaveAsync(string playerName)
    {
        var player = new PlayerPoco
        {
            PlayerName = playerName
        };

        using var database = _databaseFactory.GetDatabase();
        await database.InsertAsync(player);
    }
}
