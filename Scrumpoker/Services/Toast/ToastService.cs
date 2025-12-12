namespace Scrumpoker.Services.Toast;


public sealed class ToastService
{
    public event Action<ToastMessage>? OnShow;
    public event Action<Guid>? OnDismiss;

    private readonly Queue<ToastMessage> _pending = new();

    public void ShowInfo(string title, string? message = null, int timeoutMs = 3000)
        => Show(ToastLevel.Info, title, message, timeoutMs);

    public void ShowSuccess(string title, string? message = null, int timeoutMs = 3000)
        => Show(ToastLevel.Success, title, message, timeoutMs);

    public void ShowWarning(string title, string? message = null, int timeoutMs = 4000)
        => Show(ToastLevel.Warning, title, message, timeoutMs);

    public void ShowError(string title, string? message = null, int timeoutMs = 5000)
        => Show(ToastLevel.Error, title, message, timeoutMs);

    public void Dismiss(Guid id) => OnDismiss?.Invoke(id);

    public void ShowOnNextPage(ToastLevel level, string title, string? message = null, int timeoutMs = 4000)
    {
        var toast = new ToastMessage(
            Id: Guid.NewGuid(),
            Level: level,
            Title: title,
            Message: message,
            CreatedAt: DateTimeOffset.UtcNow,
            TimeoutMs: timeoutMs
        );

        _pending.Enqueue(toast);
    }

    public void FlushPending()
    {
        while (_pending.Count > 0)
        {
            OnShow?.Invoke(_pending.Dequeue());
        }
    }

    private void Show(ToastLevel level, string title, string? message, int timeoutMs)
    {
        var toast = new ToastMessage(
            Id: Guid.NewGuid(),
            Level: level,
            Title: title,
            Message: message,
            CreatedAt: DateTimeOffset.UtcNow,
            TimeoutMs: timeoutMs
        );

        OnShow?.Invoke(toast);
    }
}