package de.rsev.utilities.crypto;

import java.security.GeneralSecurityException;
import java.security.MessageDigest;
import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

public final class HamacProvider implements ICryptoProvider {
    private final byte[] key;
    public HamacProvider(byte[] key) { if (key == null || key.length == 0) throw new IllegalArgumentException("key is required"); this.key = key.clone(); }
    @Override public String algorithm() { return "HAMAC-SHA256"; }
    @Override public byte[] compute(byte[] data) { try { Mac mac = Mac.getInstance("HmacSHA256"); mac.init(new SecretKeySpec(key, "HmacSHA256")); return mac.doFinal(data); } catch (GeneralSecurityException exception) { throw new IllegalStateException(exception); } }
    @Override public boolean verify(byte[] data, byte[] signature) { return MessageDigest.isEqual(compute(data), signature); }
}
