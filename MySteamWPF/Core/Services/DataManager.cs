using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySteamWPF.Core.Data;
using MySteamWPF.Core.Models;

namespace MySteamWPF.Core.Services;

public static class DataManager
{
    private static IServiceProvider _serviceProvider = null!;
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public static List<User> LoadUsers()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return context.Users
            .Include(u => u.UserGames)
                .ThenInclude(ug => ug.Game)
                    .ThenInclude(g => g.Tags)
            .Include(u => u.HiddenGames)
                .ThenInclude(ug => ug.Game)
            .Include(u => u.Ratings)
                .ThenInclude(r => r.Game)
            .ToList();
    }
    
    public static void AddUser(User user)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Users.Add(user);
        context.SaveChanges();
    }
    
    public static void AddComment(Comment comment)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Comments.Add(comment);
        context.SaveChanges();
    }
    
    public static void UpdateUser(User user)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Users.Update(user);
        context.SaveChanges();
    }
    
    public static void UpdateGame(Game game)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Games.Update(game);
        context.SaveChanges();
    }
    
    public static List<Game> LoadGames()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return context.Games
            .Include(g => g.Tags)
            .Include(g => g.Ratings)
                .ThenInclude(r => r.User)
            .ToList();
    }
    
    public static List<Comment> LoadComments()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return context.Comments
            .Include(c => c.Author)
            .ToList();
    }
    
    public static void SaveAll()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.SaveChanges();
    }
    
    public static (List<User> Users, List<Game> Games, List<Comment> Comments) LoadAll()
    {
        return (
            LoadUsers(),
            LoadGames(),
            LoadComments()
        );
    }
}
