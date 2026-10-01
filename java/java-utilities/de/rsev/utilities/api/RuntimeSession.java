package de.rsev.utilities.api;

import java.time.Instant;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

public class RuntimeSession {
    private final String sessionId;
    private Instant expiresAt;
    private final Map<String, Object> metadata;
    public RuntimeSession(String sessionId, Instant expiresAt, Map<String, Object> metadata) { this.sessionId = sessionId; this.expiresAt = expiresAt; this.metadata = metadata == null ? Map.of() : Map.copyOf(metadata); }
    public RuntimeSession(String sessionId) { this(sessionId, null, Map.of()); }
    public String getSessionId() { return sessionId; }
    public Instant getExpiresAt() { return expiresAt; }
    public Map<String, Object> getMetadata() { return metadata; }
    public boolean isExpired() { return expiresAt != null && expiresAt.isBefore(Instant.now()); }
    public CompletableFuture<Void> renewAsync() { return CompletableFuture.completedFuture(null); }
}
