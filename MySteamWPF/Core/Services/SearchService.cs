using MySteamWPF.Core.Models;

namespace MySteamWPF.Core.Services;

/// <summary>
/// Provides methods to search for games by title and tags.
/// </summary>
public static class SearchService
{
    /// <summary>
    /// Searches the global library for games by part of the title (case-insensitive).
    /// </summary>
    public static List<Game> SearchByName(string namePart)
    {
        if (string.IsNullOrWhiteSpace(namePart))
            return [];

        if (DataManager.Games != null)
            return DataManager.Games
                .Where(g => g.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase))
                .ToList();
        return [];
    }

    /// <summary>
    /// Searches the global library for games by tag (case-insensitive).
    /// </summary>
    public static List<Game> SearchByTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return [];

        if (DataManager.Games != null)
            return DataManager.Games
                .Where(g => g.Tags != null &&
                            g.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        return [];
    }

    /// <summary>
    /// Searches games by name or tag at the same time.
    /// </summary>
    public static List<Game> Search(string query)
    {
        var byName = SearchByName(query);
        var byTag = SearchByTag(query);
        return byName
            .Concat(byTag)
            .DistinctBy(g => g.Id)
            .ToList();
    }

    /// <summary>
    /// Searches for games in the user's library by part of the title.
    /// </summary>
    public static List<Game> SearchUserLibraryByName(User user, string namePart)
    {
        if (string.IsNullOrWhiteSpace(namePart))
            return [];

        if (DataManager.Games != null)
            return DataManager.Games
                .Where(g => user.Games.Contains(g.Name) &&
                            g.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase))
                .ToList();
        return [];
    }

    /// <summary>
    /// Searches for games in the user's library by tag.
    /// </summary>
    public static List<Game> SearchUserLibraryByTag(User user, string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return [];

        if (DataManager.Games != null)
            return DataManager.Games
                .Where(g => user.Games.Contains(g.Name) &&
                            g.Tags != null &&
                            g.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        return [];
    }
}
