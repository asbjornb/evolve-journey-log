using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories;

public interface IDatabaseFactory
{
    IDatabase GetDatabase();
}
