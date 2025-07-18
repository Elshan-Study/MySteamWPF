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
    public List<string> Tags { get; set; } = [];
    public List<string> Comments { get; set; } = []; //id of comments
    public Dictionary<string, int> Ratings { get; set; } = new();
    public string ImagePath { get; set; } = string.Empty;

    public double AverageRating =>
        Ratings.Count == 0 ? 0 : Math.Round(Ratings.Values.Average(), 2);
}