namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents the platform user: id, login, name, email, password, balance and list of games.
/// </summary>
public class User(string id, string login, string name, string email, string password)
{
    public string Id { get; init; } = id; 
    
    public string AvatarPath {get; set;} = "Images/Avatars/DefaultAvatar.jpg";
    public string Login { get; set; } = login;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;

    public decimal Balance { get; set; } = 0;
    public List<string> Games { get; set; } = []; //id of game
    public List<string> HiddenGames { get; set; } = [];//id of game
}