namespace MySteamWPF.Core.Models;

/// <summary>
/// Represents the platform user: id, login, name, email, password, balance and list of games.
/// </summary>
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AvatarPath { get; set; } = "Images/Avatars/DefaultAvatar.jpg";
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public bool IsGaben { get; set; } = false;

    public List<UserGame> UserGames { get; set; } = new();
    public List<UserGame> HiddenGames { get; set; } = new();
    public ICollection<GameRating> Ratings { get; set; } = new List<GameRating>();
}