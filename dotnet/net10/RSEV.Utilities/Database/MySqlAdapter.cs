using System;
using System.Data.Common;
using RSEV.Utilities.Connection;

namespace RSEV.Utilities.Database;

public sealed class MySqlAdapter : IDatabaseAdapter
{
    public string Provider => "mysql";
    public DbConnection CreateConnection(ConnectionConfiguration configuration) => Create(configuration);
    private static DbConnection Create(ConnectionConfiguration configuration)
    {
        var connection = DbProviderFactories.GetFactory(configuration.ProviderInvariantName).CreateConnection()
            ?? throw new InvalidOperationException("The configured MySQL provider returned no connection.");
        connection.ConnectionString = configuration.ConnectionString;
        return connection;
    }
}
