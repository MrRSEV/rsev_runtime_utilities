namespace RSEV.Utilities.Api;

public sealed class ApiResponse
{
    public int StatusCode { get; init; }
    public IReadOnlyDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    public byte[] Body { get; init; } = Array.Empty<byte>();
    public bool IsSuccess => StatusCode is >= 200 and < 300;
}
