using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Scrumpoker.Components.Pages;

public partial class Index
{
    [Inject] private IJSRuntime Js { get; set; }
 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Js.InvokeVoidAsync("startCoins");
        }
    }
    
    private void Create()
    {
        var room = Store.CreateRoom(Guid.NewGuid().ToString());
        Nav.NavigateTo($"/room/{room.RoomId}");
    }
}