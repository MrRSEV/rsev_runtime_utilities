package de.rsev.utilities.api;

import java.time.Duration;

public record RetryPolicy(int maxAttempts, Duration delay) {
    public RetryPolicy { if (maxAttempts < 1) throw new IllegalArgumentException("maxAttempts must be positive"); if (delay.isNegative()) throw new IllegalArgumentException("delay must not be negative"); }
    public static RetryPolicy defaults() { return new RetryPolicy(3, Duration.ofMillis(200)); }
}
