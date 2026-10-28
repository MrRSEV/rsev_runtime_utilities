using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RSEV.Utilities.Session;

public sealed class JwtTokenProvider : IAccessTokenProvider
{
    private readonly byte[] _secret;
    public JwtTokenProvider(byte[] secret) => _secret = secret is { Length: > 0 } ? secret.ToArray() : throw new ArgumentException("A signing secret is required.", nameof(secret));

    public AccessToken Create(string subject, TimeSpan lifetime)
    {
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("A subject is required.", nameof(subject));
        var expires = DateTimeOffset.UtcNow.Add(lifetime);
        var header = Encode(new { alg = "HS256", typ = "JWT" });
        var payload = Encode(new { sub = subject, exp = expires.ToUnixTimeSeconds() });
        var unsigned = header + "." + payload;
        var signature = Base64Url(HMACSHA256.HashData(_secret, Encoding.UTF8.GetBytes(unsigned)));
        return new AccessToken { Value = unsigned + "." + signature, ExpiresAt = expires };
    }

    public bool Validate(AccessToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        var parts = token.Value.Split('.');
        if (parts.Length != 3 || token.IsExpired) return false;
        try
        {
            var expected = HMACSHA256.HashData(_secret, Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]));
            return CryptographicOperations.FixedTimeEquals(expected, Decode(parts[2]));
        }
        catch (FormatException) { return false; }
    }

    private static string Encode(object value) => Base64Url(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));
    private static string Base64Url(byte[] value) => Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static byte[] Decode(string value) => Convert.FromBase64String(value.Replace('-', '+').Replace('_', '/') + new string('=', (4 - value.Length % 4) % 4));
}
