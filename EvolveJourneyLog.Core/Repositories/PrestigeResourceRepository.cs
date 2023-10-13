using EvolveJourneyLog.Core.Repositories.DatabaseHelpers;
using EvolveJourneyLog.Core.Repositories.Models;
using EvolveJourneyLog.Core.Repositories.Pocos;

namespace EvolveJourneyLog.Core.Repositories;

public class PrestigeResourceRepository
{
    private readonly IDatabaseFactory _databaseFactory;

    public PrestigeResourceRepository(IDatabaseFactory dbFactory)
    {
        _databaseFactory = dbFactory;
    }

    public async Task<SaveResult> SaveAsync(int saveId, int aiCore, int antiPlasmid, int artifact, int bloodStone, double darkEnergy, int harmonyCrystal, int phage, int plasmid)
    {
        var prestigeResources = new PrestigeResourcePoco(saveId, aiCore, antiPlasmid, artifact, bloodStone, darkEnergy, harmonyCrystal, phage, plasmid);
        using var database = _databaseFactory.GetDatabase();

        await database.InsertAsync(prestigeResources);
        return SaveResult.Success;
    }
}
