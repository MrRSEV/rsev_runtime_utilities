using System;
using System.Data.Common;
using RSEV.Utilities.Connection;

namespace RSEV.Utilities.Database;

public sealed class SqlServerAdapter : IDatabaseAdapter
{
    public string Provider => "mssql";
    public DbConnection CreateConnection(ConnectionConfiguration configuration)
    {
        var connection = DbProviderFactories.GetFactory(configuration.ProviderInvariantName).CreateConnection()
            ?? throw new InvalidOperationException("The configured SQL Server provider returned no connection.");
        connection.ConnectionString = configuration.ConnectionString;
        return connection;
    }
}
