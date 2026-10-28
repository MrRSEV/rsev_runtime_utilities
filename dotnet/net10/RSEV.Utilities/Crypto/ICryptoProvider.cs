namespace RSEV.Utilities.Crypto;

public interface ICryptoProvider
{
    string Algorithm { get; }
    byte[] Compute(byte[] data);
    bool Verify(byte[] data, byte[] signature);
}
