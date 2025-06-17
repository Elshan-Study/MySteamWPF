namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents the platform user: id, login, name, email, password, balance and list of games.
/// </summary>
public class User(string id, string login, string name, string email, string password)
{
    public string Id { get; init; } = id;
    public string Login { get; init; } = login;
    public string Name { get; set; } = name;
    public string Email { get; init; } = email;
    public string Password { get; init; } = password;

    public decimal Balance { get; set; } = 0;
    public List<string> Games { get; set; } = [];
    public List<string> HiddenGames { get; set; } = [];
}