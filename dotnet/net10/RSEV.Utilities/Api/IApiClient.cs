namespace RSEV.Utilities.Api;

public interface IApiClient
{
    Task<ApiResponse> SendAsync(ApiRequest request, CancellationToken cancellationToken = default);
}
