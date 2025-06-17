// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MySteam.Models;
// using MySteam.Services;
// using MySteamWPF.Core.Data;
//
// namespace MySteam.UI.Pages;
//
// public static class GameCatalogue
// {
//     public static bool Show()
//     {
//         while (true)
//         {
//             Console.Clear();
//             Console.WriteLine("Game Catalogue");
//             Console.WriteLine("----------------------------------------------");
//
//             DisplayGames(Database.Games);
//
//             Console.WriteLine("----------------------------------------------");
//             Console.WriteLine("Commands:");
//             Console.WriteLine("1. Search by name");
//             Console.WriteLine("2. Search by tag");
//             Console.WriteLine("3. Show game page by number");
//             Console.WriteLine("Other - Exit");
//             Console.Write("Choice: ");
//
//             var input = Console.ReadLine();
//
//             switch (input)
//             {
//                 case "1":
//                     SearchGameByName();
//                     break;
//                 case "2":
//                     SearchGameByTag();
//                     break;
//                 case "3":
//                     ShowGamePageByNumber(Database.Games);
//                     break;
//                 default:
//                     return false;
//             }
//         }
//     }
//
//     private static void DisplayGames(List<Game>? games)
//     {
//         if (games == null || games.Count == 0)
//         {
//             Console.WriteLine("No games found.");
//             return;
//         }
//
//         for (int i = 0; i < games.Count; i++)
//         {
//             var game = games[i];
//             Console.WriteLine($"{i + 1}. {game.Name} ({Math.Round(game.AverageRating, 2)}/5)");
//             Console.WriteLine(game.Description);
//             if (game.Tags is { Count: > 0 })
//                 Console.WriteLine(string.Join(" ", game.Tags.Select(t => $"|{t}|")));
//             Console.WriteLine();
//         }
//     }
//
//     private static void SearchGameByName()
//     {
//         Console.Clear();
//         Console.Write("Enter game name or part of it: ");
//         var input = Console.ReadLine()?.Trim();
//
//         if (input != null)
//         {
//             var foundGames = SearchService.SearchByName(input);
//
//             ShowSearchResults(foundGames);
//         }
//     }
//
//     private static void SearchGameByTag()
//     {
//         Console.Clear();
//         Console.Write("Enter tag: ");
//         var input = Console.ReadLine()?.Trim();
//
//         if (input != null)
//         {
//             var foundGames = SearchService.SearchByTag(input);
//
//             ShowSearchResults(foundGames);
//         }
//     }
//
//     private static void ShowSearchResults(List<Game>? foundGames)
//     {
//         Console.Clear();
//         Console.WriteLine("Search results:");
//         Console.WriteLine("----------------------------------------------");
//
//         if (foundGames is { Count: 0 })
//         {
//             Console.WriteLine("No games found.");
//             Pause();
//             return;
//         }
//
//         DisplayGames(foundGames);
//
//         Console.WriteLine("----------------------------------------------");
//         Console.WriteLine("Enter game number to view details, or press Enter to go back:");
//         Console.Write("Choice: ");
//         var input = Console.ReadLine();
//
//         if (int.TryParse(input, out int choice) && choice >= 1 && foundGames != null && choice <= foundGames.Count)
//         {
//             GamePage.CurrentGame = foundGames[choice - 1];
//             Console.Clear();
//             GamePage.Show();
//             Pause();
//         }
//     }
//
//     private static void ShowGamePageByNumber(List<Game>? games)
//     {
//         Console.Clear();
//         Console.WriteLine("Enter game number to view details:");
//         var input = Console.ReadLine();
//
//         if (int.TryParse(input, out int choice) && choice >= 1 && games != null && choice <= games.Count)
//         {
//             GamePage.CurrentGame = games[choice - 1];
//             Console.Clear();
//             GamePage.Show();
//             Pause();
//         }
//         else
//         {
//             Console.WriteLine("Invalid choice.");
//             Pause();
//         }
//     }
//
//     private static void Pause(string message = "Press any key to continue...")
//     {
//         Console.WriteLine(message);
//         Console.ReadKey(true);
//     }
// }
