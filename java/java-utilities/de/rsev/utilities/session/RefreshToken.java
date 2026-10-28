package de.rsev.utilities.session;

import java.time.Instant;

public record RefreshToken(String value, Instant expiresAt) {
    public boolean isExpired() { return !Instant.now().isBefore(expiresAt); }
}
