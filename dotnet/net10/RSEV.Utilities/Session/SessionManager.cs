using System.Security.Cryptography;

namespace RSEV.Utilities.Session;

public sealed class SessionManager : ISessionManager
{
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly TimeSpan _accessTokenLifetime;
    private readonly TimeSpan _refreshTokenLifetime;

    public SessionManager(IAccessTokenProvider tokenProvider, TimeSpan? accessTokenLifetime = null, TimeSpan? refreshTokenLifetime = null)
    {
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _accessTokenLifetime = accessTokenLifetime ?? TimeSpan.FromHours(1);
        _refreshTokenLifetime = refreshTokenLifetime ?? TimeSpan.FromDays(30);
    }

    public SessionContext Create(string subject) => new()
    {
        Subject = subject,
        AccessToken = _tokenProvider.Create(subject, _accessTokenLifetime),
        RefreshToken = new RefreshToken { Value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), ExpiresAt = DateTimeOffset.UtcNow.Add(_refreshTokenLifetime) }
    };

    public bool IsValid(SessionContext context) => context is not null && context.AccessToken is not null && _tokenProvider.Validate(context.AccessToken) && context.RefreshToken is { IsExpired: false };
}
