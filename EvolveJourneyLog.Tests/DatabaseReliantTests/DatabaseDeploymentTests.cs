namespace EvolveJourneyLog.Tests.DatabaseReliantTests;
using System.Threading.Tasks;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using PetaPoco;
using Xunit;

[Collection("Database collection"), Trait("Category", "Simple")]
public sealed class DatabaseDeploymentTests : IDisposable
{
    private readonly IDatabaseFactory _databaseFactory;

    public DatabaseDeploymentTests(DatabaseSetup databaseSetup)
    {
        _databaseFactory = new DatabaseFactory(databaseSetup.ConnectionString);
    }

    public void Dispose()
    {
        // Reset the table after each test
        using var database = _databaseFactory.GetDatabase();
        database.Execute("DELETE FROM [SomeSchema].[SomeTable];");
    }

    [Fact]
    public async Task CanWriteAndReadFromDatabase()
    {
        using var database = _databaseFactory.GetDatabase();

        // Insert data
        database.Execute("INSERT INTO [SomeSchema].[SomeTable] (Id) VALUES (1);");

        // Read data
        var id = await database.ExecuteScalarAsync<int>("SELECT Id FROM [SomeSchema].[SomeTable] WHERE Id = 1;");

        Assert.Equal(1, id);
    }
}

internal interface IDatabaseFactory
{
    IDatabase GetDatabase();
}

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
