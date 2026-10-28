using System.Data.Common;
using RSEV.Utilities.Database;

namespace RSEV.Utilities.Connection;

public sealed class ConnectionFactory
{
    private readonly IDatabaseAdapter _adapter;

    public ConnectionFactory(IDatabaseAdapter adapter) => _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));

    public DbConnection Create(ConnectionConfiguration configuration)
    {
        configuration.Validate();
        return _adapter.CreateConnection(configuration);
    }
}
