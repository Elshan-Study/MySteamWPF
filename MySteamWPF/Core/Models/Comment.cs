namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents a comment left by a user on a game page.
/// </summary>
public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorId { get; set; } = string.Empty;
    public User? Author { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
}