namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents a comment left by a user on a game page.
/// </summary>
public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string GameId { get; set; } = string.Empty;
    public Game Game { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
}