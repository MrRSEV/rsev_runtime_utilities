package de.rsev.utilities.api;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.concurrent.CompletableFuture;

public final class RuntimeApiClient {
    private final String host;
    private final int port;
    private final HttpClient client;
    private RuntimeSession session;

    public RuntimeApiClient(String host, int port) { this.host = host; this.port = port; this.client = HttpClient.newBuilder().connectTimeout(Duration.ofSeconds(10)).build(); }
    public RuntimeApiClient setSession(RuntimeSession session) { this.session = session; return this; }
    public CompletableFuture<RuntimeResponse> callAsync(String route, String payload) { return sendAsync(new RuntimeRequestBuilder().withRoute(route).withPayload(payload).build()); }

    public CompletableFuture<RuntimeResponse> sendAsync(RuntimeRequest request) {
        RuntimeRequest effective = request;
        Map<String, String> headers = new LinkedHashMap<>(request.headers());
        String sessionId = request.sessionId();
        if (session != null) { sessionId = session.getSessionId(); headers.put("X-Session-Id", sessionId); }
        if (request.body().length > 0) headers.putIfAbsent("Content-Type", "application/json");
        headers.put("Content-Length", Integer.toString(request.body().length));
        HttpRequest.Builder builder = HttpRequest.newBuilder(URI.create("http://" + host + ":" + port + request.route())).timeout(Duration.ofSeconds(30)).POST(HttpRequest.BodyPublishers.ofByteArray(request.body()));
        headers.forEach(builder::header);
        effective = new RuntimeRequest(request.route(), request.body(), headers, sessionId);
        return client.sendAsync(builder.build(), HttpResponse.BodyHandlers.ofByteArray()).thenApply(response -> new RuntimeResponse(response.statusCode(), response.body(), response.headers().map().entrySet().stream().collect(java.util.stream.Collectors.toMap(Map.Entry::getKey, e -> String.join(",", e.getValue()))), response.statusCode() >= 400 ? new String(response.body(), java.nio.charset.StandardCharsets.UTF_8) : null));
    }
}
