using System.Security.Cryptography;

namespace RSEV.Utilities.Crypto;

public sealed class KeyManager
{
    public byte[] Generate(int size = 32)
    {
        if (size < 16) throw new ArgumentOutOfRangeException(nameof(size), "Keys must contain at least 16 bytes.");
        return RandomNumberGenerator.GetBytes(size);
    }
}
