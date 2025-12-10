namespace Scrumpoker.Components.Data;

public class RoomModel
{
    public string RoomId { get; set; }
    public string Name { get; set; }
    public Dictionary<string, Player> Players { get; set; } = [];
    public bool CardsRevealed { get; set; }
}