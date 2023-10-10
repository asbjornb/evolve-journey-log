using PetaPoco;

namespace EvolveJourneyLog.Core.Repositories;

public class DatabaseFactory : IDatabaseFactory
{
    private readonly string _connectionString;

    public DatabaseFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDatabase GetDatabase()
    {
        return new Database(_connectionString, "System.Data.SqlClient"); // Assumes SQL Server. Change the provider name if using a different DB.
    }
}
