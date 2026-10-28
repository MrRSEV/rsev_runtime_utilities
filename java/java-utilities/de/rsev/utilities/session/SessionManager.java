package de.rsev.utilities.session;

import java.security.SecureRandom;
import java.time.Duration;
import java.time.Instant;
import java.util.Base64;
import java.util.Objects;

public final class SessionManager implements ISessionManager {
    private final IAccessTokenProvider tokenProvider;
    private final Duration accessLifetime;
    private final Duration refreshLifetime;
    private final SecureRandom random = new SecureRandom();
    public SessionManager(IAccessTokenProvider tokenProvider) { this(tokenProvider, Duration.ofHours(1), Duration.ofDays(30)); }
    public SessionManager(IAccessTokenProvider tokenProvider, Duration accessLifetime, Duration refreshLifetime) { this.tokenProvider = Objects.requireNonNull(tokenProvider); this.accessLifetime = accessLifetime; this.refreshLifetime = refreshLifetime; }
    @Override public SessionContext create(String subject) { byte[] bytes = new byte[32]; random.nextBytes(bytes); return new SessionContext(subject, tokenProvider.create(subject, accessLifetime), new RefreshToken(Base64.getUrlEncoder().withoutPadding().encodeToString(bytes), Instant.now().plus(refreshLifetime))); }
    @Override public boolean isValid(SessionContext context) { return context != null && tokenProvider.validate(context.accessToken()) && !context.refreshToken().isExpired(); }
}
