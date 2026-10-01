package de.rsev.utilities.api;

import java.nio.charset.StandardCharsets;
import java.util.LinkedHashMap;
import java.util.Map;

public final class RuntimeRequestBuilder {
    private String route = "/";
    private byte[] body = new byte[0];
    private final Map<String, String> headers = new LinkedHashMap<>();
    private String sessionId;

    public RuntimeRequestBuilder withRoute(String route) { this.route = route; return this; }
    public RuntimeRequestBuilder withPayload(String payload) { this.body = payload == null ? new byte[0] : payload.getBytes(StandardCharsets.UTF_8); headers.putIfAbsent("Content-Type", "application/json"); return this; }
    public RuntimeRequestBuilder withPayload(byte[] payload) { this.body = payload == null ? new byte[0] : payload.clone(); return this; }
    public RuntimeRequestBuilder withPayload(Object payload) { return withPayload(payload == null ? null : payload.toString()); }
    public RuntimeRequestBuilder withHeader(String name, String value) { headers.put(name, value); return this; }
    public RuntimeRequestBuilder bindSession(RuntimeSession session) { sessionId = session == null ? null : session.getSessionId(); return this; }
    public RuntimeRequest build() { return new RuntimeRequest(route, body, headers, sessionId); }
    public byte[] buildBytes() {
        RuntimeRequest request = build();
        Map<String, String> requestHeaders = new LinkedHashMap<>(request.headers());
        requestHeaders.putIfAbsent("Content-Length", Integer.toString(request.body().length));
        if (request.sessionId() != null) requestHeaders.put("X-Session-Id", request.sessionId());
        StringBuilder result = new StringBuilder("POST ").append(request.route()).append(" HTTP/1.1\r\n");
        requestHeaders.forEach((name, value) -> result.append(name).append(": ").append(value).append("\r\n"));
        result.append("\r\n");
        byte[] header = result.toString().getBytes(StandardCharsets.ISO_8859_1);
        byte[] payload = request.body();
        byte[] combined = new byte[header.length + payload.length];
        System.arraycopy(header, 0, combined, 0, header.length);
        System.arraycopy(payload, 0, combined, header.length, payload.length);
        return combined;
    }
}
