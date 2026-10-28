package de.rsev.utilities.crypto;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public final class Sha256Provider implements ICryptoProvider {
    @Override public String algorithm() { return "SHA-256"; }
    @Override public byte[] compute(byte[] data) { try { return MessageDigest.getInstance("SHA-256").digest(data); } catch (NoSuchAlgorithmException exception) { throw new IllegalStateException(exception); } }
    @Override public boolean verify(byte[] data, byte[] signature) { return MessageDigest.isEqual(compute(data), signature); }
}
