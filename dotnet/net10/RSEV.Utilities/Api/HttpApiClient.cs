namespace RSEV.Utilities.Api;

public sealed class HttpApiClient : IApiClient
{
    private readonly HttpClient _client;
    private readonly RetryPolicy _retryPolicy;
    private readonly RateLimitPolicy _rateLimitPolicy;

    public HttpApiClient(HttpClient client, RetryPolicy? retryPolicy = null, RateLimitPolicy? rateLimitPolicy = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _retryPolicy = retryPolicy ?? new RetryPolicy();
        _rateLimitPolicy = rateLimitPolicy ?? new RateLimitPolicy();
    }

    public Task<ApiResponse> SendAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return _retryPolicy.ExecuteAsync(() => SendOnceAsync(request, cancellationToken), response => response.StatusCode >= 500, cancellationToken);
    }

    private async Task<ApiResponse> SendOnceAsync(ApiRequest request, CancellationToken cancellationToken)
    {
        if (!_rateLimitPolicy.TryAcquire()) throw new InvalidOperationException("The API rate limit has been reached.");
        using var message = new HttpRequestMessage(request.Method, request.Uri) { Content = new ByteArrayContent(request.Body) };
        foreach (var header in request.Headers) message.Headers.TryAddWithoutValidation(header.Key, header.Value);
        using var response = await _client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        return new ApiResponse
        {
            StatusCode = (int)response.StatusCode,
            Headers = response.Headers.Concat(response.Content.Headers).ToDictionary(pair => pair.Key, pair => string.Join(",", pair.Value)),
            Body = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false)
        };
    }
}
