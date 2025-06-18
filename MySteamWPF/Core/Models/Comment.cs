namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents a comment left by a user on a game page.
/// </summary>
public class Comment(string authorName, string authorId, string gameId, string message)
{
    // var comment = new Comment(user.Name, user.Id, game.Id, "Крутая игра!");
    // DataManager.Comments.Add(comment);
    // DataManager.SaveAll();
    
    //А чтобы получить комментарии к конкретной игре:
    //var gameComments = DataManager.Comments.Where(c => c.GameId == game.Id).ToList();

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AuthorName { get; set; } = authorName;
    public string AuthorId { get; set; } = authorId;
    public string GameId { get; set; } = gameId;
    public string Message { get; set; } = message;
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
}