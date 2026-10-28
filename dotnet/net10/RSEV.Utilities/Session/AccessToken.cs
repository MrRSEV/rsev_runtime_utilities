namespace RSEV.Utilities.Session;

public sealed class AccessToken
{
    public string Value { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
}
