package de.rsev.utilities.api;

import java.util.concurrent.CompletableFuture;

public interface IRuntimeEndpoint {
    String getRoute();
    EndpointResponse handle(EndpointRequest request);
    default CompletableFuture<EndpointResponse> handleAsync(EndpointRequest request) {
        return CompletableFuture.supplyAsync(() -> handle(request));
    }
}
