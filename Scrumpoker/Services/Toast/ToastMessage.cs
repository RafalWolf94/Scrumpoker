namespace Scrumpoker.Services.Toast;

public sealed record ToastMessage(
    Guid Id,
    ToastLevel Level,
    string Title,
    string? Message,
    DateTimeOffset CreatedAt,
    int TimeoutMs
);