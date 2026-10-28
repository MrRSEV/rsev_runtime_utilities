using System.Data.Common;
using RSEV.Utilities.Connection;

namespace RSEV.Utilities.Database;

/// <summary>Provider-neutral database adapter.</summary>
public interface IDatabaseAdapter
{
    string Provider { get; }
    DbConnection CreateConnection(ConnectionConfiguration configuration);
}
