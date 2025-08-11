namespace MySteamWPF.Core.Models;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    
    public ICollection<GameTag> GameTags { get; set; } = new List<GameTag>();
}
