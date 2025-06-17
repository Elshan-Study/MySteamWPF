// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MySteam.Services;
// using MySteam.Utilities;
// using MySteamWPF.Core.Data;
//
// namespace MySteam.UI.Pages;
//
// public static class UserDashboard
// {
//     public static bool Show()
//     {
//         while (true)
//         {
//             Console.Clear();
//             var user = AccountManager.CurrentUser!;
//             Console.WriteLine($"{user.Name}'s Dashboard");
//             Console.WriteLine("----------------------------------------------");
//             Console.WriteLine($"Login: {user.Login}");
//             Console.WriteLine($"Email: {user.Email}");
//             Console.WriteLine($"Balance: {user.Balance:C}");
//             Console.WriteLine("----------------------------------------------");
//
//             DisplayGames(user.Games, "Owned Games");
//             Console.WriteLine("----------------------------------------------");
//
//             Console.WriteLine("Commands: ");
//             Console.WriteLine("1. Hide game");
//             Console.WriteLine("2. Show hidden game");
//             Console.WriteLine("3. Top up balance");
//             Console.WriteLine("4. View Game Catalogue");
//             Console.WriteLine("5. Search my games");
//             Console.WriteLine("6. Search hidden games");
//             Console.WriteLine("Any other key to Exit");
//
//             Console.Write("Your choice: ");
//             var input = Console.ReadLine();
//
//             switch (input)
//             {
//                 case "1":
//                     HideGame();
//                     break;
//                 case "2":
//                     UnhideGame();
//                     break;
//                 case "3":
//                     TopUpBalance();
//                     break;
//                 case "4":
//                     Console.Clear();
//                     GameCatalogue.Show();
//                     break;
//                 case "5":
//                     SearchGames(AccountManager.CurrentUser!.Games, "Your Games");
//                     break;
//                 case "6":
//                     SearchGames(AccountManager.CurrentUser!.HiddenGames, "Hidden Games");
//                     break;
//                 default:
//                     return false;
//             }
//         }
//     }
//
//     private static void DisplayGames(List<string> gameNames, string title)
//     {
//         Console.WriteLine(title + ":");
//         if (Database.Games != null)
//         {
//             var games = Database.Games
//                 .Where(g => gameNames.Contains(g.Name))
//                 .ToList();
//
//             if (!games.Any())
//             {
//                 Console.WriteLine("No games to show.");
//                 return;
//             }
//
//             foreach (var game in games)
//             {
//                 Console.WriteLine($"- {game.Name} ({Math.Round(game.AverageRating, 2)}/5)");
//                 Console.WriteLine(game.Description);
//                 if (game.Tags != null)
//                     Console.WriteLine(string.Join(" ", game.Tags.Select(t => $"|{t}|")));
//                 Console.WriteLine();
//             }
//         }
//     }
//
//     private static void HideGame()
//     {
//         Console.Clear();
//         Console.WriteLine("Enter the name of the game to hide:");
//         var input = Console.ReadLine()?.Trim();
//         var user = AccountManager.CurrentUser!;
//
//         if (string.IsNullOrWhiteSpace(input) || !user.Games.Contains(input))
//         {
//             Console.WriteLine("Game not found in your collection.");
//         }
//         else
//         {
//             user.Games.Remove(input);
//             user.HiddenGames.Add(input);
//             Console.WriteLine("Game hidden successfully.");
//             Logger.Log($"{user.Login} hid the game: {input}");
//         }
//
//         Pause();
//     }
//
//     private static void UnhideGame()
//     {
//         Console.Clear();
//         DisplayGames(AccountManager.CurrentUser!.HiddenGames, "Hidden Games");
//
//         Console.WriteLine("Enter the name of the game to unhide:");
//         var input = Console.ReadLine()?.Trim();
//         var user = AccountManager.CurrentUser;
//
//         if (string.IsNullOrWhiteSpace(input) || !user.HiddenGames.Contains(input))
//         {
//             Console.WriteLine("Game not found in hidden list.");
//         }
//         else
//         {
//             user.HiddenGames.Remove(input);
//             user.Games.Add(input);
//             Console.WriteLine("Game restored to library.");
//             Logger.Log($"{user.Login} unhid the game: {input}");
//         }
//
//         Pause();
//     }
//
//     private static void TopUpBalance()
//     {
//         Console.Clear();
//         Console.Write("Enter amount to top up: ");
//         var input = Console.ReadLine();
//
//         if (decimal.TryParse(input, out var amount) && amount > 0)
//         {
//             var user = AccountManager.CurrentUser!;
//             user.Balance += amount;
//             Console.WriteLine($"Balance updated. New balance: {user.Balance:C}");
//             Logger.Log($"{user.Login} topped up balance by {amount:C}");
//         }
//         else
//         {
//             Console.WriteLine("Invalid amount.");
//         }
//
//         Pause();
//     }
//
//     private static void SearchGames(List<string> gameNames, string listTitle)
//     {
//         Console.Clear();
//         Console.Write($"Enter part of the game name to search in {listTitle}: ");
//         var input = Console.ReadLine()?.Trim();
//
//         if (string.IsNullOrWhiteSpace(input))
//         {
//             Console.WriteLine("Search cancelled.");
//             Pause();
//             return;
//         }
//
//         if (Database.Games != null)
//         {
//             var foundGames = Database.Games
//                 .Where(g => gameNames.Contains(g.Name) &&
//                             g.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
//                 .ToList();
//
//             Console.Clear();
//             Console.WriteLine($"Search results in {listTitle}:");
//             Console.WriteLine("----------------------------------------------");
//
//             if (!foundGames.Any())
//             {
//                 Console.WriteLine("No games found.");
//             }
//             else
//             {
//                 foreach (var game in foundGames)
//                 {
//                     Console.WriteLine($"- {game.Name} ({Math.Round(game.AverageRating, 2)}/5)");
//                     Console.WriteLine(game.Description);
//                     if (game.Tags != null)
//                         Console.WriteLine(string.Join(" ", game.Tags.Select(t => $"|{t}|")));
//                     Console.WriteLine();
//                 }
//             }
//         }
//
//         Pause();
//     }
//
//     private static void Pause(string message = "Press any key to continue...")
//     {
//         Console.WriteLine(message);
//         Console.ReadKey(true);
//     }
// }
