package de.rsev.utilities.api;

import java.nio.charset.StandardCharsets;
import java.util.LinkedHashMap;
import java.util.Map;

public final class EndpointResponse {
    private final int statusCode;
    private final byte[] body;
    private final Map<String, String> headers;

    public EndpointResponse(int statusCode, byte[] body, Map<String, String> headers) {
        this.statusCode = statusCode;
        this.body = body == null ? new byte[0] : body.clone();
        this.headers = headers == null ? new LinkedHashMap<>() : new LinkedHashMap<>(headers);
    }

    public EndpointResponse(int statusCode, byte[] body) { this(statusCode, body, Map.of()); }
    public static EndpointResponse fromText(int statusCode, String text) { return new EndpointResponse(statusCode, text.getBytes(StandardCharsets.UTF_8), Map.of("Content-Type", "text/plain; charset=utf-8")); }
    public static EndpointResponse fromJson(int statusCode, String json) { return new EndpointResponse(statusCode, json.getBytes(StandardCharsets.UTF_8), Map.of("Content-Type", "application/json; charset=utf-8")); }
    public int getStatusCode() { return statusCode; }
    public byte[] getBody() { return body.clone(); }
    public Map<String, String> getHeaders() { return Map.copyOf(headers); }
}
