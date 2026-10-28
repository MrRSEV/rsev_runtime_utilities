package de.rsev.utilities.api;

import java.net.http.HttpClient;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

public final class HttpApiClient implements IApiClient {
    private final HttpClient client;
    private final RetryPolicy retryPolicy;
    private final RateLimitPolicy rateLimitPolicy;
    public HttpApiClient(HttpClient client) { this(client, RetryPolicy.defaults(), new RateLimitPolicy(60, Duration.ofMinutes(1))); }
    public HttpApiClient(HttpClient client, RetryPolicy retryPolicy, RateLimitPolicy rateLimitPolicy) { this.client = client; this.retryPolicy = retryPolicy; this.rateLimitPolicy = rateLimitPolicy; }
    @Override public CompletableFuture<ApiResponse> sendAsync(ApiRequest request) { return sendAttempt(request, 1); }
    private CompletableFuture<ApiResponse> sendAttempt(ApiRequest request, int attempt) {
        if (!rateLimitPolicy.tryAcquire()) return CompletableFuture.failedFuture(new IllegalStateException("API rate limit reached"));
        return client.sendAsync(request.toHttpRequest().build(), HttpResponse.BodyHandlers.ofByteArray()).thenApply(response -> new ApiResponse(response.statusCode(), response.headers().map().entrySet().stream().collect(java.util.stream.Collectors.toMap(Map.Entry::getKey, entry -> String.join(",", entry.getValue()))), response.body())).thenCompose(response -> {
            if (response.statusCode() >= 500 && attempt < retryPolicy.maxAttempts()) {
                return CompletableFuture.supplyAsync(() -> Boolean.TRUE, CompletableFuture.delayedExecutor(retryPolicy.delay().toMillis(), java.util.concurrent.TimeUnit.MILLISECONDS)).thenCompose(ignored -> sendAttempt(request, attempt + 1));
            }
            return CompletableFuture.completedFuture(response);
        });
    }
}
