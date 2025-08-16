using System.IO;
using System.Text.Json;
using MySteamWPF.Core.Models;

namespace MySteamWPF.Core.Data;

/// <summary>
/// Initializes the primary data for the database when it is created.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();
        
        if (context.Games.Any())
            return; 
        
        var seedPath = Path.Combine(AppContext.BaseDirectory,  "Core", "Data", "seed.json");
        if (!File.Exists(seedPath))
            throw new FileNotFoundException("Seed file not found.", seedPath);

        var jsonString = File.ReadAllText(seedPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var seedData = JsonSerializer.Deserialize<SeedData>(jsonString, options)
                       ?? throw new Exception("Failed to deserialize seed data.");

        // --- Tags ---
        context.Tags.AddRange(seedData.Tags);
        context.SaveChanges();

        // --- Games ---
        context.Games.AddRange(seedData.Games);
        context.SaveChanges();

        // --- GameTags ---
        foreach (var gt in seedData.GameTags)
        {
            var game = context.Games.First(g => g.Id == gt.GameId);
            var tag = context.Tags.First(t => t.Id == gt.TagId);
            context.GameTags.Add(new GameTag { Game = game, GameId = game.Id, Tag = tag, TagId = tag.Id });
        }
        context.SaveChanges();

        // --- Users ---
        context.Users.AddRange(seedData.Users);
        context.SaveChanges();

        // --- Comments ---
        foreach (var comment in seedData.Comments)
        {
            comment.Game = context.Games.First(g => g.Id == comment.GameId);
            comment.User = context.Users.First(u => u.Id == comment.UserId);
            context.Comments.Add(comment);
        }
        context.SaveChanges();

        // --- Ratings ---
        foreach (var rating in seedData.Ratings)
        {
            rating.Game = context.Games.First(g => g.Id == rating.GameId);
            rating.User = context.Users.First(u => u.Id == rating.UserId);
            context.GameRating.Add(rating);
        }
        context.SaveChanges();

        // --- UserGames ---
        foreach (var ug in seedData.UserGames)
        {
            ug.Game = context.Games.First(g => g.Id == ug.GameId);
            ug.User = context.Users.First(u => u.Id == ug.UserId);
            context.UserGames.Add(ug);
        }
        context.SaveChanges();
    }
}
