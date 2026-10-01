package de.rsev.utilities.api;

import java.nio.charset.StandardCharsets;
import java.util.Map;

public record RuntimeResponse(int statusCode, byte[] body, Map<String, String> headers, String errorMessage) {
    public RuntimeResponse { body = body == null ? new byte[0] : body.clone(); headers = headers == null ? Map.of() : Map.copyOf(headers); }
    @Override public byte[] body() { return body.clone(); }
    public boolean isError() { return statusCode >= 400; }
    public String getBodyAsText() { return new String(body, StandardCharsets.UTF_8); }
}
