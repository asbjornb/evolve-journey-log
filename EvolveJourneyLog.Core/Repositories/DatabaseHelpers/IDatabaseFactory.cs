using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories.DatabaseHelpers;

public interface IDatabaseFactory
{
    IDatabase GetDatabase();
}
