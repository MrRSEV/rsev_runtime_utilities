using System;
using System.Data.Common;
using RSEV.Utilities.Connection;

namespace RSEV.Utilities.Database;

public sealed class PostgreSqlAdapter : IDatabaseAdapter
{
    public string Provider => "postgresql";
    public DbConnection CreateConnection(ConnectionConfiguration configuration)
    {
        var connection = DbProviderFactories.GetFactory(configuration.ProviderInvariantName).CreateConnection()
            ?? throw new InvalidOperationException("The configured PostgreSQL provider returned no connection.");
        connection.ConnectionString = configuration.ConnectionString;
        return connection;
    }
}
