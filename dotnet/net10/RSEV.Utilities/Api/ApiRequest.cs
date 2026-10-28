namespace RSEV.Utilities.Api;

public sealed class ApiRequest
{
    public HttpMethod Method { get; init; } = HttpMethod.Get;
    public Uri Uri { get; init; } = new("http://localhost");
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public byte[] Body { get; init; } = Array.Empty<byte>();
}
