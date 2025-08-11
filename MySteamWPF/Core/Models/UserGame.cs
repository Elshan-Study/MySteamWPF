namespace MySteamWPF.Core.Models;

public class UserGame
{
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public string GameId { get; set; } = null!;
    public Game Game { get; set; } = null!;
    
    public bool IsHidden { get; set; } = false;
}