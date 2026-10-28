using System.Security.Cryptography;

namespace RSEV.Utilities.Crypto;

public sealed class Sha256Provider : ICryptoProvider
{
    public string Algorithm => "SHA-256";
    public byte[] Compute(byte[] data) => SHA256.HashData(data);
    public bool Verify(byte[] data, byte[] signature) => CryptographicOperations.FixedTimeEquals(Compute(data), signature);
}
