namespace RSEV.Utilities.Api;

public sealed class RateLimitPolicy
{
    private readonly object _sync = new();
    private readonly Queue<DateTimeOffset> _requests = new();
    public int RequestsPerWindow { get; init; } = 60;
    public TimeSpan Window { get; init; } = TimeSpan.FromMinutes(1);

    public bool TryAcquire()
    {
        lock (_sync)
        {
            var cutoff = DateTimeOffset.UtcNow - Window;
            while (_requests.Count > 0 && _requests.Peek() <= cutoff) _requests.Dequeue();
            if (_requests.Count >= RequestsPerWindow) return false;
            _requests.Enqueue(DateTimeOffset.UtcNow);
            return true;
        }
    }
}
