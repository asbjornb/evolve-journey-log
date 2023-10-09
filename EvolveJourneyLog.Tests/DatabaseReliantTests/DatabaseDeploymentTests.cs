namespace EvolveJourneyLog.Tests.DatabaseReliantTests;
using System.Threading.Tasks;
using EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;
using FluentAssertions;
using PetaPoco;
using Xunit;

[Collection("Database collection"), Trait("Category", "Slow")]
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
        database.Execute("DELETE FROM [gamedata].[Player];");
    }

    [Fact]
    public async Task CanWriteAndReadFromDatabase()
    {
        using var database = _databaseFactory.GetDatabase();

        // Insert data
        database.Execute("INSERT INTO [gamedata].[Player] (PlayerName) VALUES ('SomeName');");

        // Read data
        var playerName = await database.ExecuteScalarAsync<string>("SELECT TOP 1 PlayerName FROM [gamedata].[Player];");

        playerName.Should().Be("SomeName");
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
