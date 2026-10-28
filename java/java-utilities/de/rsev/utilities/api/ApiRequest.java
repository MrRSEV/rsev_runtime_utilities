package de.rsev.utilities.api;

import java.net.URI;
import java.net.http.HttpRequest;
import java.util.Map;

public record ApiRequest(String method, URI uri, Map<String, String> headers, byte[] body) {
    public ApiRequest { method = method == null || method.isBlank() ? "GET" : method; headers = headers == null ? Map.of() : Map.copyOf(headers); body = body == null ? new byte[0] : body.clone(); }
    public byte[] body() { return body.clone(); }
    public HttpRequest.Builder toHttpRequest() { HttpRequest.Builder builder = HttpRequest.newBuilder(uri).method(method, HttpRequest.BodyPublishers.ofByteArray(body)); headers.forEach(builder::header); return builder; }
}
