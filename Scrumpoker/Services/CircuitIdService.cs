namespace Scrumpoker.Services;

public class CircuitIdService
{
    private static AsyncLocal<string> _currentCircuitId = new();

    public string CircuitId
    {
        get => _currentCircuitId.Value;
        set => _currentCircuitId.Value = value;
    }
}