using System.Collections.Concurrent;
using System.Data.Common;

namespace RSEV.Utilities.Connection;

public sealed class ConnectionPool : IDisposable
{
    private readonly ConcurrentBag<DbConnection> _connections = new();
    private readonly int _maximumSize;
    private int _count;

    public ConnectionPool(int maximumSize = 10)
    {
        if (maximumSize < 1) throw new ArgumentOutOfRangeException(nameof(maximumSize));
        _maximumSize = maximumSize;
    }

    public DbConnection Rent(Func<DbConnection> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (_connections.TryTake(out var connection)) { Interlocked.Decrement(ref _count); return connection; }
        return factory();
    }

    public bool Return(DbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (Interlocked.Increment(ref _count) > _maximumSize) { Interlocked.Decrement(ref _count); connection.Dispose(); return false; }
        _connections.Add(connection);
        return true;
    }

    public void Clear()
    {
        while (_connections.TryTake(out var connection)) { Interlocked.Decrement(ref _count); connection.Dispose(); }
    }

    public void Dispose() => Clear();
}
