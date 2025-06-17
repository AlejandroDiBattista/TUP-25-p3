using Microsoft.JSInterop;
using Cliente.Constants;

namespace Cliente.Services;

public class DebounceService
{
    private readonly Dictionary<string, CancellationTokenSource> _pendingTasks = new();
    
    public async Task DebounceAsync(string key, Func<Task> action, int delayMs = AppConstants.DEBOUNCE_DELAY_MS)
    {

        if (_pendingTasks.TryGetValue(key, out var existingCts))
        {
            existingCts.Cancel();
            _pendingTasks.Remove(key);
        }
        

        var cts = new CancellationTokenSource();
        _pendingTasks[key] = cts;
        
        try
        {
            await Task.Delay(delayMs, cts.Token);
            await action();
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _pendingTasks.Remove(key);
            cts.Dispose();
        }
    }
    
    public void CancelPending(string key)
    {
        if (_pendingTasks.TryGetValue(key, out var cts))
        {
            cts.Cancel();
            _pendingTasks.Remove(key);
        }
    }
    
    public void CancelAll()
    {
        foreach (var cts in _pendingTasks.Values)
        {
            cts.Cancel();
        }
        _pendingTasks.Clear();
    }
}
