package de.rsev.utilities.api;

import java.util.concurrent.CompletableFuture;

public interface IApiClient {
    CompletableFuture<ApiResponse> sendAsync(ApiRequest request);
}
