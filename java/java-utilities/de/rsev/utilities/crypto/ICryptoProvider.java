package de.rsev.utilities.crypto;

public interface ICryptoProvider {
    String algorithm();
    byte[] compute(byte[] data);
    boolean verify(byte[] data, byte[] signature);
}
