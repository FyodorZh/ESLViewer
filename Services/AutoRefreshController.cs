using ESLViewer.Models;

namespace ESLViewer.Services;

/// <summary>
/// Reusable auto-refresh logic shared between Panel and DashboardPanel.
/// Owns the CancellationTokenSource and PeriodicTimer loop.
/// </summary>
public sealed class AutoRefreshController : IAsyncDisposable
{
    private CancellationTokenSource? _cts;
    private Func<CancellationToken, Task>? _callback;

    public bool IsEnabled { get; private set; }
    public AutoRefreshInterval Interval { get; private set; } = AutoRefreshInterval.Seconds5;

    /// <summary>Initialize from a saved model state (does NOT start the loop).</summary>
    public void Initialize(bool enabled, AutoRefreshInterval interval)
    {
        IsEnabled = enabled;
        Interval = interval;
    }

    /// <summary>Toggle auto-refresh on or off, starting/stopping the loop as needed.</summary>
    public void Toggle(bool enabled, Func<CancellationToken, Task> callback)
    {
        IsEnabled = enabled;
        _callback = callback;
        if (enabled)
            Start(callback);
        else
            Stop();
    }

    /// <summary>Change the interval, restarting the loop if currently active.</summary>
    public void ChangeInterval(AutoRefreshInterval interval)
    {
        Interval = interval;
        if (IsEnabled && _callback is not null)
        {
            Stop();
            Start(_callback);
        }
    }

    /// <summary>Start the refresh loop unconditionally.</summary>
    public void Start(Func<CancellationToken, Task> callback)
    {
        _callback = callback;
        Stop();
        _cts = new CancellationTokenSource();
        _ = LoopAsync(callback, _cts.Token);
    }

    /// <summary>Stop any running refresh loop.</summary>
    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public TimeSpan GetTimeSpan() => Interval switch
    {
        AutoRefreshInterval.Seconds3  => TimeSpan.FromSeconds(3),
        AutoRefreshInterval.Seconds5  => TimeSpan.FromSeconds(5),
        AutoRefreshInterval.Seconds10 => TimeSpan.FromSeconds(10),
        AutoRefreshInterval.Minute1   => TimeSpan.FromMinutes(1),
        AutoRefreshInterval.Minute10  => TimeSpan.FromMinutes(10),
        AutoRefreshInterval.Hour1     => TimeSpan.FromHours(1),
        _                             => TimeSpan.FromSeconds(5)
    };

    private async Task LoopAsync(Func<CancellationToken, Task> callback, CancellationToken token)
    {
        using var timer = new PeriodicTimer(GetTimeSpan());
        try
        {
            while (await timer.WaitForNextTickAsync(token))
                await callback(token);
        }
        catch (OperationCanceledException) { }
    }

    public ValueTask DisposeAsync()
    {
        Stop();
        return ValueTask.CompletedTask;
    }
}

