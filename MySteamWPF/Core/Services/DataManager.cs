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
            .ThenInclude(g => g.GameTags)
            .Include(u => u.Ratings)
            .ThenInclude(r => r.Game)
            .Include(u => u.Comments) 
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
    
    public static void AddRating(GameRating rating)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.GameRatings.Add(rating);
        context.SaveChanges();
    }

    public static void UpdateRating(GameRating rating)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.GameRatings.Update(rating);
        context.SaveChanges();
    }
    
    public static void PurchaseGame(string userId, Game game)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = context.Users
            .Include(u => u.UserGames)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
            throw new InvalidOperationException("Пользователь не найден.");

        if (user.UserGames.Any(ug => ug.GameId == game.Id))
            throw new InvalidOperationException("Вы уже приобрели эту игру.");

        if (user.Balance < game.Price)
            throw new InvalidOperationException("Недостаточно средств для покупки игры.");

        user.Balance -= game.Price;
        user.UserGames.Add(new UserGame { GameId = game.Id, UserId = user.Id });

        context.SaveChanges();
    }

    public static List<Game> LoadGames()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return context.Games
            .Include(g => g.GameTags)
            .ThenInclude(gt => gt.Tag)
            .Include(g => g.Ratings)
            .ThenInclude(r => r.User)
            .Include(g => g.Comments)
            .ThenInclude(c => c.User)
            .ToList();
    }
    
    public static List<Comment> LoadComments()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return context.Comments
            .Include(c => c.User)
            .Include(c => c.Game)
            .ToList();
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
