using System;
using System.Data.Common;
using RSEV.Utilities.Connection;

namespace RSEV.Utilities.Database;

public sealed class SqliteAdapter : IDatabaseAdapter
{
    public string Provider => "sqlite";
    public DbConnection CreateConnection(ConnectionConfiguration configuration)
    {
        var connection = DbProviderFactories.GetFactory(configuration.ProviderInvariantName).CreateConnection()
            ?? throw new InvalidOperationException("The configured SQLite provider returned no connection.");
        connection.ConnectionString = configuration.ConnectionString;
        return connection;
    }
}
