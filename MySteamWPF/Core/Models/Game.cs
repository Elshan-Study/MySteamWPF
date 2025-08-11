namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents the game model: id, name, description, price, tags, comments and ratings.
/// </summary>
public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0;
    
    public List<GameTag> Tags { get; set; } = new();
    public List<string> Comments { get; set; } = new(); //id of comments
    public ICollection<GameRating> Ratings { get; set; } = new List<GameRating>();
    public string ImagePath { get; set; } = string.Empty;

    public double AverageRating =>
        !Ratings.Any()
            ? 0
            : Math.Round(Ratings.Average(r => r.Rating), 2);
}