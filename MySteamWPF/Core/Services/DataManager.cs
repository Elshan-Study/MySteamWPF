using System.IO;
using System.Text.Json;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Core.Services;

/// <summary>
/// The class is responsible for loading and saving user, game and comment data from/to JSON files.
/// </summary>
internal static class DataManager
{
    /// <summary>
    /// The base folder where all JSON files with data are stored.
    /// </summary>
    private const string BasePath = "Core/Data";

    /// <summary>
    /// List of registered users.
    /// </summary>
    public static List<User> Users { get; private set; } = [];
    
    /// <summary>
    /// List of all games in the global library.
    /// </summary>
    public static List<Game>? Games { get; private set; } = [];
    
    /// <summary>
    /// List of all comments for all games.
    /// </summary>
    public static List<Comment> Comments { get; private set; } = [];

    /// <summary>
    /// Loads all data (users, games, comments) from the corresponding JSON files.
    /// </summary>
    public static void LoadAll()
    {
        Users = Load<List<User>>($"{BasePath}/users.json") ?? [];
        Games = Load<List<Game>>($"{BasePath}/games.json") ?? [];
        Comments = Load<List<Comment>>($"{BasePath}/comments.json") ?? [];
    }

    /// <summary>
    /// Saves all data (users, games, comments) to corresponding JSON files.
    /// </summary>
    public static void SaveAll()
    {
        Save($"{BasePath}/users.json", Users);
        Save($"{BasePath}/games.json", Games);
        Save($"{BasePath}/comments.json", Comments);
    }

    /// <summary>
    /// Loads data from the specified JSON file.
    /// </summary>
    /// <typeparam name="T">Type of data to load (e.g. List&lt;User&gt;).</typeparam>
    /// <param name="filepath">Path to file.</param>
    /// <returns>Object of type T, or null if error or file is missing.</returns>
    private static T? Load<T>(string filepath)
    {
        if (!File.Exists(filepath)) return default;
        try
        {
            var json = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "DataManager");
            return default;
        }
    }

    /// <summary>
    /// Saves the specified data to a JSON file.
    /// </summary>
    /// <typeparam name="T">The type of data to save.</typeparam>
    /// <param name="filepath">The path to the file.</param>
    /// <param name="data">The data to save.</param>
    private static void Save<T>(string filepath, T data)
    {
        var dir = Path.GetDirectoryName(filepath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir!);

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filepath, json);
    }
}