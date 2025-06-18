namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents a comment left by a user on a game page.
/// </summary>
public class Comment(string authorId,string message)
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorId { get; set; } = authorId;
    public string Message { get; set; } = message;
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
}