using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Scrumpoker.Components.Data;
using Scrumpoker.Services;

namespace Scrumpoker.Components.Pages;

public partial class Room : IDisposable
{
    [Inject] private RoomStore Store { get; set; }
    [Inject] private IJSRuntime Js { get; set; }
    [Inject] private UserConnectionTracker Tracker { get; set; }
    [Inject] private CircuitIdService CircuitIdService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    [Parameter] public string RoomId { get; set; }

    private RoomModel _room;
    private string _userName;
    private bool _joined;
    private readonly string[] _cards = ["1/2", "1", "2", "3", "5", "7", "8", "10", "13", "20", "50", "100"];
    private bool _initialized;
    private string _circuitId;
    private bool _showTooltip;
    private const string CopyStatus = "Link skopiowany do schowka";
    private const string DefaultCard = "/assets/cards/cards_bg.jpg";
    private string _hoverCard = string.Empty;
    private string _hoveredCard;
    private string _selectedCard;
    private string _selectedCardImage;

    protected override void OnInitialized()
    {
        _room = Store.Get(RoomId);
        Store.OnRoomChanged += RoomChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialized)
        {
            _initialized = true;
            var savedName = await Js.InvokeAsync<string>("localStorage.getItem", [$"scrumpoker-username-{RoomId}"]);
            if (!string.IsNullOrWhiteSpace(savedName))
            {
                _userName = savedName;
                await Join();
            }

            _initialized = true;
            StateHasChanged();
        }
    }

    private async Task Join()
    {
        if (string.IsNullOrWhiteSpace(_userName))
            return;

        if (!_room.Players.ContainsKey(_userName))
            _room.Players[_userName] = new Player { UserName = _userName };

        _circuitId = CircuitIdService.CircuitId ?? Guid.NewGuid().ToString();
        Tracker.AddConnection(_circuitId, RoomId, _userName);
        await Js.InvokeVoidAsync("localStorage.setItem", $"scrumpoker-username-{RoomId}", _userName);

        _joined = true;
        Store.NotifyRoomChanged(RoomId);
        await Js.InvokeVoidAsync("stopCoins");
        StateHasChanged();
    }

    private bool AllSelected => _room.Players.Values.All(p => p.Card != null);

    private void SelectCard(string card)
    {
        _room.Players[_userName].Card = card;
        _selectedCard = card;
        _selectedCardImage = _hoverCard;
        Store.UpdateRoom(RoomId);
        StateHasChanged();
    }

    private void Reveal()
    {
        _room.CardsRevealed = true;
        Store.UpdateRoom(RoomId);
    }

    private void Clear()
    {
        _room.CardsRevealed = false;
        _selectedCard = null;
        _selectedCardImage = null;
        foreach (var p in _room.Players.Values)
            p.Card = null;

        Store.UpdateRoom(RoomId);
    }

    private void RoomChanged(string roomId)
    {
        if (roomId == RoomId)
        {
            _room = Store.Get(RoomId);
            InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        if (!string.IsNullOrEmpty(_circuitId))
            Tracker.RemoveConnection(_circuitId);
        Store.OnRoomChanged -= RoomChanged;
    }

    private async Task CopyLink()
    {
        var url = NavigationManager.Uri;
        await Js.InvokeVoidAsync("navigator.clipboard.writeText", url);
        _showTooltip = true;
        StateHasChanged();
        await Task.Delay(2000);
        _showTooltip = false;
        StateHasChanged();
    }

    private void OnHover(string card)
    {
        _hoveredCard = card;
        _hoverCard = GetRandomCard();
    }

    private void OnLeave()
    {
        _hoveredCard = null;
        if (string.IsNullOrWhiteSpace(_selectedCard))
            _hoverCard = null;
    }

    private string GetRandomCard()
    {
        string[] types = ["clubs", "diamonds", "hearts", "spades"];
        string[] values = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "queen", "king", "ace"];
        Random rnd = new();
        var value = values[rnd.Next(values.Length)];
        var type = types[rnd.Next(types.Length)];
        var cardImage = $"/assets/cards/{value}_of_{type}.png";
        if (!cardImage.Equals(_selectedCardImage))
            return cardImage;

        return GetRandomCard();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Join();
        }
    }
}