package de.rsev.utilities.session;

import java.nio.charset.StandardCharsets;
import java.security.GeneralSecurityException;
import java.security.MessageDigest;
import java.time.Duration;
import java.time.Instant;
import java.util.Base64;
import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

public final class JwtTokenProvider implements IAccessTokenProvider {
    private final byte[] secret;
    public JwtTokenProvider(byte[] secret) { if (secret == null || secret.length == 0) throw new IllegalArgumentException("secret is required"); this.secret = secret.clone(); }
    @Override public AccessToken create(String subject, Duration lifetime) {
        if (subject == null || subject.isBlank()) throw new IllegalArgumentException("subject is required");
        Instant expires = Instant.now().plus(lifetime);
        String header = encode("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
        String payload = encode("{\"sub\":\"" + subject.replace("\"", "\\\"") + "\",\"exp\":" + expires.getEpochSecond() + "}");
        String unsigned = header + "." + payload;
        try { Mac mac = Mac.getInstance("HmacSHA256"); mac.init(new SecretKeySpec(secret, "HmacSHA256")); return new AccessToken(unsigned + "." + encode(mac.doFinal(unsigned.getBytes(StandardCharsets.UTF_8))), expires); } catch (GeneralSecurityException exception) { throw new IllegalStateException(exception); }
    }
    @Override public boolean validate(AccessToken token) {
        if (token == null || token.isExpired()) return false;
        String[] parts = token.value().split("\\.");
        if (parts.length != 3) return false;
        try { Mac mac = Mac.getInstance("HmacSHA256"); mac.init(new SecretKeySpec(secret, "HmacSHA256")); return MessageDigest.isEqual(mac.doFinal((parts[0] + "." + parts[1]).getBytes(StandardCharsets.UTF_8)), Base64.getUrlDecoder().decode(parts[2])); } catch (GeneralSecurityException | IllegalArgumentException exception) { return false; }
    }
    private static String encode(String value) { return encode(value.getBytes(StandardCharsets.UTF_8)); }
    private static String encode(byte[] value) { return Base64.getUrlEncoder().withoutPadding().encodeToString(value); }
}
