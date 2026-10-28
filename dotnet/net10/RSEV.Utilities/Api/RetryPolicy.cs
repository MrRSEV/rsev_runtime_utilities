namespace RSEV.Utilities.Api;

public sealed class RetryPolicy
{
    public int MaxAttempts { get; init; } = 3;
    public TimeSpan Delay { get; init; } = TimeSpan.FromMilliseconds(200);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, Func<T, bool> shouldRetry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(shouldRetry);
        var attempts = Math.Max(1, MaxAttempts);
        for (var attempt = 1; ; attempt++)
        {
            var result = await operation().ConfigureAwait(false);
            if (attempt >= attempts || !shouldRetry(result)) return result;
            await Task.Delay(Delay, cancellationToken).ConfigureAwait(false);
        }
    }
}
