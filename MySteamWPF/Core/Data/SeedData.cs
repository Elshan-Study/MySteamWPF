using MySteamWPF.Core.Models;

namespace MySteamWPF.Core.Data;

public class SeedData
{
    public List<Tag> Tags { get; set; } = new();
    public List<Game> Games { get; set; } = new();
    public List<GameTag> GameTags { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<GameRating> Ratings { get; set; } = new();
    public List<UserGame> UserGames { get; set; } = new();
}