package de.rsev.utilities.api;

import java.util.Map;

public record ApiResponse(int statusCode, Map<String, String> headers, byte[] body) {
    public ApiResponse { headers = headers == null ? Map.of() : Map.copyOf(headers); body = body == null ? new byte[0] : body.clone(); }
    public byte[] body() { return body.clone(); }
    public boolean isSuccess() { return statusCode >= 200 && statusCode < 300; }
}
