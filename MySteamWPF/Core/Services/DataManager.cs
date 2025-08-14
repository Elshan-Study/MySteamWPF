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
            .ThenInclude(gt => gt.Tag)
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

        var existingUser = context.Users
            .Include(u => u.UserGames)
            .ThenInclude(ug => ug.Game)
            .FirstOrDefault(u => u.Id == user.Id);

        if (existingUser == null)
            throw new InvalidOperationException("User not found");
        
        existingUser.Name = user.Name;
        existingUser.Login = user.Login;
        existingUser.Password = user.Password;
        existingUser.AvatarPath = user.AvatarPath;
        existingUser.Email = user.Email;
        existingUser.Balance = user.Balance;

        context.SaveChanges();
    }
    
    public static void AddRating(GameRating rating)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.GameRating.Add(rating);
        context.SaveChanges();
    }

    public static void UpdateRating(GameRating rating)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.GameRating.Update(rating);
        context.SaveChanges();
    }
    
    public static void UpdateUserGame(UserGame userGame)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.UserGames.Attach(userGame);
        context.Entry(userGame).Property(ug => ug.IsHidden).IsModified = true;
        context.SaveChanges();
    }
    
    public static void AddUserGame(UserGame userGame)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.UserGames.Add(userGame);
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
    
    public static void DeleteGame(string gameId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var game = context.Games
            .Include(g => g.Comments)
            .Include(g => g.Ratings)
            .Include(g => g.GameTags)
            .FirstOrDefault(g => g.Id == gameId);

        if (game == null) return;
        
        context.Comments.RemoveRange(game.Comments);
        context.GameRating.RemoveRange(game.Ratings);
        context.GameTags.RemoveRange(game.GameTags);
        var userGames = context.UserGames.Where(ug => ug.GameId == gameId).ToList();
        context.UserGames.RemoveRange(userGames);
        context.Games.Remove(game);
        context.SaveChanges();
    }

    public static void DeleteComment(string commentId)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var comment = context.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null) return;

        context.Comments.Remove(comment);
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
}
