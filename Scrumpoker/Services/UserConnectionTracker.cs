using System.Collections.Concurrent;

public class UserConnectionTracker
{
    private readonly ConcurrentDictionary<string, (string RoomId, string UserName)> _connections = new();
    private readonly ConcurrentDictionary<string, string> _pendingUserNames = new();
    public event Action<string, string>? OnConnectionRemoved;
    
    public void AddConnection(string circuitId, string roomId, string userName)
    {
        _connections[circuitId] = (roomId, userName);
        _pendingUserNames.TryRemove(circuitId, out _);
    }

    public void RemoveConnection(string circuitId)
    {
        if (_connections.TryRemove(circuitId, out var info))
        {
            OnConnectionRemoved?.Invoke(info.RoomId, info.UserName);
        }
        _pendingUserNames.TryRemove(circuitId, out _);
    }

    public bool TryGetConnection(string circuitId, out (string RoomId, string UserName) info) =>
        _connections.TryGetValue(circuitId, out info);
    
}