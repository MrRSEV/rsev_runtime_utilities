using System.Data.Common;

namespace RSEV.Utilities.Connection;

public sealed class ConnectionManager : IConnectionManager
{
    private readonly ConnectionFactory _factory;
    private readonly ConnectionPool _pool;
    private readonly ConnectionConfiguration _configuration;

    public ConnectionManager(ConnectionFactory factory, ConnectionConfiguration configuration)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configuration.Validate();
        _pool = new ConnectionPool(_configuration.MaximumPoolSize);
    }

    public DbConnection Open(ConnectionConfiguration configuration) => _pool.Rent(() => _factory.Create(configuration));
    public void Return(DbConnection connection) => _pool.Return(connection);
    public void Dispose() => _pool.Dispose();
}
