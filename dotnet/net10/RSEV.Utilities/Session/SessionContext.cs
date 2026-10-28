namespace RSEV.Utilities.Session;

public sealed class SessionContext
{
    public string Subject { get; init; } = string.Empty;
    public AccessToken? AccessToken { get; init; }
    public RefreshToken? RefreshToken { get; init; }
}
