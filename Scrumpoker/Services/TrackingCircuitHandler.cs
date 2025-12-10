using Microsoft.AspNetCore.Components.Server.Circuits;
using Scrumpoker.Components.Data;

namespace Scrumpoker.Services;


public class TrackingCircuitHandler : CircuitHandler
{
    private readonly UserConnectionTracker _tracker;
    private readonly RoomStore _roomStore;
    private readonly IServiceProvider _serviceProvider;

    public TrackingCircuitHandler(UserConnectionTracker tracker, RoomStore roomStore, IServiceProvider serviceProvider)
    {
        _tracker = tracker;
        _roomStore = roomStore;
        _serviceProvider = serviceProvider;
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var circuitIdService = scope.ServiceProvider.GetRequiredService<CircuitIdService>();
        circuitIdService.CircuitId = circuit.Id;
        

        return Task.CompletedTask;
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (_tracker.TryGetConnection(circuit.Id, out var info))
        {
            var room = _roomStore.Get(info.RoomId);
            if (room != null)
            {
                room.Players.Remove(info.UserName);
                _roomStore.NotifyRoomChanged(info.RoomId);
            }
        }
        _tracker.RemoveConnection(circuit.Id);

        return Task.CompletedTask;
    }
}