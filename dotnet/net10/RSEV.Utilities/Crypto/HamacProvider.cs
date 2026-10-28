using System.Security.Cryptography;

namespace RSEV.Utilities.Crypto;

/// <summary>Manifest-compatible HAMAC provider backed by HMAC-SHA256.</summary>
public sealed class HamacProvider : ICryptoProvider
{
    private readonly byte[] _key;
    public HamacProvider(byte[] key) => _key = key is { Length: > 0 } ? key.ToArray() : throw new ArgumentException("A key is required.", nameof(key));
    public string Algorithm => "HAMAC-SHA256";
    public byte[] Compute(byte[] data) => HMACSHA256.HashData(_key, data);
    public bool Verify(byte[] data, byte[] signature) => CryptographicOperations.FixedTimeEquals(Compute(data), signature);
}
