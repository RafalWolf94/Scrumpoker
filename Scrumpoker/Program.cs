using Microsoft.AspNetCore.Components.Server.Circuits;
using Scrumpoker.Components;
using Scrumpoker.Components.Data;
using Scrumpoker.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => 
{
    options.DetailedErrors = true;
});
builder.Services.AddSingleton<RoomStore>();
builder.Services.AddSingleton<UserConnectionTracker>();
builder.Services.AddScoped<CircuitIdService>();
builder.Services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();