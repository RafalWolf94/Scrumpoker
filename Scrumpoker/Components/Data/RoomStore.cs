using System.Collections.Concurrent;

namespace Scrumpoker.Components.Data;

public class RoomStore
{
    private readonly ConcurrentDictionary<string, RoomModel> _rooms = new();

    public event Action<string> OnRoomChanged;

    public RoomModel CreateRoom(string name)
    {
        var id = Guid.NewGuid().ToString("N")[..6];
        var room = new RoomModel { RoomId = id, Name = name };
        _rooms[id] = room;
        OnRoomChanged?.Invoke(id);
        return room;
    }

    public RoomModel Get(string id)
    {
        _rooms.TryGetValue(id, out var room);
        return room;
    }

    public void NotifyRoomChanged(string roomId)
    {
        OnRoomChanged?.Invoke(roomId);
    }
    
    public void UpdateRoom(string id)
    {
        OnRoomChanged?.Invoke(id);
    }
    
    public RoomStore(UserConnectionTracker tracker)
    {
        tracker.OnConnectionRemoved += (roomId, userName) =>
        {
            if (!_rooms.TryGetValue(roomId, out var room)) return;
            if (room.Players.Remove(userName))
            {
                NotifyRoomChanged(roomId);
            }
        };
    }
}