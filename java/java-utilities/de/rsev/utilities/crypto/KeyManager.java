package de.rsev.utilities.crypto;

import java.security.SecureRandom;

public final class KeyManager {
    private final SecureRandom random = new SecureRandom();
    public byte[] generate(int size) { if (size < 16) throw new IllegalArgumentException("size must be at least 16"); byte[] key = new byte[size]; random.nextBytes(key); return key; }
}
