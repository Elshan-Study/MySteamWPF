namespace MySteamWPF.Core.Models;

public class GameTag
{
    public string GameId { get; set; } = null!;
    public Game Game { get; set; } = null!;

    public int TagId { get; set; } 
    public Tag Tag { get; set; } = null!;
}
