namespace MySteamWPF.Core.Models;

public class GameRating
{
    public string GameId { get; set; } = null!;
    public Game Game { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public int Rating { get; set; }
}