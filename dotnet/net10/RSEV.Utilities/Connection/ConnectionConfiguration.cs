namespace RSEV.Utilities.Connection;

public sealed class ConnectionConfiguration
{
    public string ProviderInvariantName { get; init; } = string.Empty;
    public string ConnectionString { get; init; } = string.Empty;
    public int MinimumPoolSize { get; init; } = 1;
    public int MaximumPoolSize { get; init; } = 10;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProviderInvariantName)) throw new ArgumentException("A provider invariant name is required.", nameof(ProviderInvariantName));
        if (string.IsNullOrWhiteSpace(ConnectionString)) throw new ArgumentException("A connection string is required.", nameof(ConnectionString));
        if (MinimumPoolSize < 0 || MaximumPoolSize < 1 || MinimumPoolSize > MaximumPoolSize) throw new ArgumentOutOfRangeException(nameof(MaximumPoolSize));
    }
}
