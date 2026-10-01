package de.rsev.utilities.api;

import java.util.Map;

public record RuntimeRequest(String route, byte[] body, Map<String, String> headers, String sessionId) {
    public RuntimeRequest { body = body == null ? new byte[0] : body.clone(); headers = headers == null ? Map.of() : Map.copyOf(headers); }
    @Override public byte[] body() { return body.clone(); }
}
