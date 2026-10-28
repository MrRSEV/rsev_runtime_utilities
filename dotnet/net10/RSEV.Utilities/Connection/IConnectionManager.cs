using System.Data.Common;

namespace RSEV.Utilities.Connection;

public interface IConnectionManager : IDisposable
{
    DbConnection Open(ConnectionConfiguration configuration);
    void Return(DbConnection connection);
}
