package de.rsev.utilities.api;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.charset.StandardCharsets;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/** Dependency-free HTTP/1.1-style TCP host for runtime endpoints. */
public final class RuntimeApiHost implements AutoCloseable {
    private final String host;
    private int port;
    private final RuntimeEndpointController endpoints = new RuntimeEndpointController();
    private volatile ServerSocket server;
    private ExecutorService executor;
    private Thread acceptThread;

    public RuntimeApiHost() { this("127.0.0.1", 0); }
    public RuntimeApiHost(String host, int port) { this.host = host; this.port = port; }
    public int getPort() { return port; }
    public RuntimeEndpointController getEndpoints() { return endpoints; }
    public boolean register(IRuntimeEndpoint endpoint) { return endpoints.register(endpoint); }
    public boolean unregister(String route) { return endpoints.unregister(route); }

    public synchronized boolean start() throws IOException {
        if (server != null) return true;
        server = new ServerSocket(port, 50, java.net.InetAddress.getByName(host));
        port = server.getLocalPort();
        executor = Executors.newCachedThreadPool(r -> { Thread thread = new Thread(r, "rsev-api-client"); thread.setDaemon(true); return thread; });
        acceptThread = new Thread(this::acceptLoop, "rsev-api-accept");
        acceptThread.setDaemon(true);
        acceptThread.start();
        return true;
    }

    public synchronized void stop() {
        if (server == null) return;
        try { server.close(); } catch (IOException ignored) { }
        if (executor != null) executor.shutdownNow();
        server = null;
    }

    @Override public void close() { stop(); }

    private void acceptLoop() {
        while (server != null) {
            try { Socket socket = server.accept(); executor.submit(() -> handle(socket)); }
            catch (IOException error) { if (server != null) break; }
        }
    }

    private void handle(Socket socket) {
        try (Socket connection = socket) {
            BufferedInputStream input = new BufferedInputStream(connection.getInputStream());
            String headerText = readHeaders(input);
            String[] lines = headerText.split("\\r\\n");
            String[] requestLine = lines[0].split(" ", 3);
            Map<String, String> headers = new LinkedHashMap<>();
            int length = 0;
            for (int i = 1; i < lines.length; i++) {
                int separator = lines[i].indexOf(':');
                if (separator > 0) { String key = lines[i].substring(0, separator).trim(); String value = lines[i].substring(separator + 1).trim(); headers.put(key, value); if (key.equalsIgnoreCase("Content-Length")) length = Integer.parseInt(value); }
            }
            byte[] body = input.readNBytes(length);
            IRuntimeEndpoint endpoint = endpoints.resolve(requestLine[1].split("\\?", 2)[0]);
            EndpointResponse response = endpoint == null ? EndpointResponse.fromText(404, "Not Found") : endpoint.handle(new EndpointRequest(requestLine[0], requestLine[1], headers, body));
            send(connection.getOutputStream(), response);
        } catch (Exception error) {
            try { send(socket.getOutputStream(), EndpointResponse.fromText(500, error.getMessage() == null ? "Internal Server Error" : error.getMessage())); } catch (IOException ignored) { }
        }
    }

    private static String readHeaders(BufferedInputStream input) throws IOException {
        StringBuilder result = new StringBuilder();
        int matched = 0;
        while (matched < 4) { int value = input.read(); if (value < 0) throw new IOException("incomplete request"); result.append((char) value); matched = value == (matched == 0 ? '\r' : matched == 1 ? '\n' : matched == 2 ? '\r' : '\n') ? matched + 1 : 0; if (result.length() > 1024 * 1024) throw new IOException("request headers too large"); }
        return result.substring(0, result.length() - 4);
    }

    private static void send(OutputStream output, EndpointResponse response) throws IOException {
        Map<String, String> headers = new LinkedHashMap<>(response.getHeaders());
        headers.putIfAbsent("Content-Length", Integer.toString(response.getBody().length));
        headers.putIfAbsent("Connection", "close");
        StringBuilder header = new StringBuilder("HTTP/1.1 ").append(response.getStatusCode()).append(" ").append(reason(response.getStatusCode())).append("\\r\\n");
        headers.forEach((key, value) -> header.append(key).append(": ").append(value).append("\\r\\n"));
        header.append("\\r\\n");
        output.write(header.toString().getBytes(StandardCharsets.ISO_8859_1));
        output.write(response.getBody());
        output.flush();
    }

    private static String reason(int status) { return status == 200 ? "OK" : status == 404 ? "Not Found" : status >= 500 ? "Internal Server Error" : "Response"; }
}
