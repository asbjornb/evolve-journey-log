using System.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using Testcontainers.MsSql;

namespace EvolveJourneyLog.Tests.DatabaseReliantTests.Setup;

public sealed class DatabaseSetup : IDisposable
{
    private const string TestDatabaseName = "EvolveJourneyLogTests";
    private const string DacPacPath = "./EvolveJourneyLog.Database.Build.dacpac";
    private IAsyncDisposable? _disposable;

    public string ConnectionString { get; }
    public bool CleanUpLocal { get; set; }

    public DatabaseSetup()
    {
        CleanUpLocal = true;
        if (Environment.GetEnvironmentVariable("CI") == "true") //Check which variable we can use when relevant
        {
            ConnectionString = InitializeDocker().GetAwaiter().GetResult();
            return;
        }
        ConnectionString = $"Server=localhost;Database={TestDatabaseName};Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite";
        InitializeLocal(ConnectionString);
    }

    private async Task<string> InitializeDocker()
    {
        var testDatabase = new MsSqlBuilder().Build();
        _disposable = testDatabase;
        await testDatabase.StartAsync().ConfigureAwait(false);
        var connectionString = testDatabase.GetConnectionString().Replace("Database=master", $"Database={TestDatabaseName}"); //Connect to new test database rather than master
        Deploy(connectionString, upgradeExisting: false);
        return connectionString;
    }

    private static void InitializeLocal(string connectionString)
    {
        Deploy(connectionString, true);
    }

    private static void Deploy(string connectionString, bool upgradeExisting)
    {
        var dacpac = DacPackage.Load(DacPacPath);
        var dacOptions = new DacDeployOptions { CreateNewDatabase = true };
        var dacServiceInstance = new DacServices(connectionString);
        dacServiceInstance.Deploy(dacpac, TestDatabaseName, upgradeExisting, dacOptions);
    }

    public void Dispose()
    {
        if (_disposable is not null)
        {
            _disposable?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        else if (CleanUpLocal) // Can be nice to be able to inspect the database after tests
        {
            //Use default database since you can't drop from inside
            using var connection = new SqlConnection("Server=localhost;Integrated Security=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite");
            connection.Open();
            using var command = new SqlCommand($"ALTER DATABASE {TestDatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {TestDatabaseName};", connection);
            command.ExecuteNonQuery();
        }
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseSetup> { }
