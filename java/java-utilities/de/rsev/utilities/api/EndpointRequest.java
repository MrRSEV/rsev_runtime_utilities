package de.rsev.utilities.api;

import java.nio.charset.StandardCharsets;
import java.util.Collections;
import java.util.Map;

public final class EndpointRequest {
    private final String method;
    private final String route;
    private final Map<String, String> headers;
    private final byte[] body;

    public EndpointRequest(String method, String route, Map<String, String> headers, byte[] body) {
        this.method = method;
        this.route = route;
        this.headers = headers == null ? Map.of() : Map.copyOf(headers);
        this.body = body == null ? new byte[0] : body.clone();
    }

    public String getMethod() { return method; }
    public String getRoute() { return route; }
    public Map<String, String> getHeaders() { return Collections.unmodifiableMap(headers); }
    public byte[] getBody() { return body.clone(); }
    public String getBodyAsText() { return new String(body, StandardCharsets.UTF_8); }
}
