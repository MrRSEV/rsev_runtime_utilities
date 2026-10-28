package de.rsev.utilities.api;

import java.time.Duration;
import java.time.Instant;
import java.util.ArrayDeque;
import java.util.Deque;

public final class RateLimitPolicy {
    private final int requestsPerWindow;
    private final Duration window;
    private final Deque<Instant> requests = new ArrayDeque<>();
    public RateLimitPolicy(int requestsPerWindow, Duration window) { if (requestsPerWindow < 1) throw new IllegalArgumentException("requestsPerWindow must be positive"); this.requestsPerWindow = requestsPerWindow; this.window = window; }
    public synchronized boolean tryAcquire() { Instant cutoff = Instant.now().minus(window); while (!requests.isEmpty() && requests.peekFirst().isBefore(cutoff)) requests.removeFirst(); if (requests.size() >= requestsPerWindow) return false; requests.addLast(Instant.now()); return true; }
}
