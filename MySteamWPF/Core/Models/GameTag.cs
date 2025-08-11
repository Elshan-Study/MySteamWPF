namespace MySteamWPF.Core.Models;

public class GameTag
{
    public string GameId { get; set; }
    public Game Game { get; set; }

    public string TagId { get; set; }
    public Tag Tag { get; set; }
}
