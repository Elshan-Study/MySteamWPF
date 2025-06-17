// using System;
// using MySteamWPF.Core.Services;
// using MySteamWPF.Core.UI.Pages;
// using MySteamWPF.Core.Data;
// using MySteamWPF.Core.Exceptions;
// using MySteamWPF.Core.Utilities;
//
// namespace MySteam.UI;
//
// public static class MenuManager
// {
//     public static void ShowMainMenu()
//     {
//         Database.LoadAll();
//         AccountManager.Notify += msg => Console.WriteLine($"[Info] {msg}");
//
//         while (true)
//         {
//             Console.Clear();
//
//             Console.WriteLine("Main Menu:");
//             Console.WriteLine("----------------------------------------------");
//             Console.WriteLine(AccountManager.CurrentUser == null
//                 ? "You are not logged in."
//                 : $"Logged in as: {AccountManager.CurrentUser.Login}");
//             Console.WriteLine("----------------------------------------------");
//
//             Console.WriteLine(AccountManager.CurrentUser == null ? "1. Login" : "1. Logout");
//             Console.WriteLine("2. Registration");
//             Console.WriteLine("3. Game Catalogue");
//             Console.WriteLine("Other - Exit");
//
//             Console.Write("Choice: ");
//             var input = Console.ReadLine();
//
//             switch (input)
//             {
//                 case "1":
//                     if (AccountManager.CurrentUser == null)
//                     {
//                         if (LoginMenu())
//                         {
//                             Console.Clear();
//                             UserDashboard.Show();
//                             Pause();
//                         }
//                     }
//                     else
//                     {
//                         LogoutMenu();
//                     }
//                     break;
//
//                 case "2":
//                     RegistrationMenu();
//                     break;
//
//                 case "3":
//                     Console.Clear();
//                     GameCatalogue.Show();
//                     Pause();
//                     break;
//
//                 default:
//                     Database.SaveAll();
//                     return;
//             }
//         }
//     }
//
//     private static void RegistrationMenu()
//     {
//         Console.Clear();
//
//         Console.WriteLine("Registration");
//         Console.WriteLine("----------------------------------------------");
//
//         Console.Write("Enter login: ");
//         var login = Console.ReadLine();
//         if (!Validator.IsValidLogin(login))
//         {
//             Console.WriteLine("Login must be at least 3 characters and contain only letters, numbers, dashes or underscores.");
//             Pause();
//             return;
//         }
//
//         Console.Write("Enter email: ");
//         var email = Console.ReadLine();
//         if (!Validator.IsValidEmail(email))
//         {
//             Console.WriteLine("Invalid email format.");
//             Pause();
//             return;
//         }
//
//         Console.Write("Enter name: ");
//         var name = Console.ReadLine();
//         if (!Validator.IsValidName(name))
//         {
//             Console.WriteLine("Name must be at least 2 characters.");
//             Pause();
//             return;
//         }
//
//         Console.Write("Enter password: ");
//         var password = Console.ReadLine();
//         if (!Validator.IsValidPassword(password))
//         {
//             Console.WriteLine("Password must be at least 8 characters and contain both letters and digits.");
//             Pause();
//             return;
//         }
//
//         try
//         {
//             AccountManager.Register(login!, name!, email!, password!);
//             Console.WriteLine("Registration successful.");
//         }
//         catch (UserExistsException ex)
//         {
//             Console.WriteLine(ex.Message);
//             Logger.LogException(ex, "RegistrationMenu");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Unexpected error: " + ex.Message);
//             Logger.LogException(ex, "RegistrationMenu");
//         }
//
//         Pause();
//     }
//
//     private static bool LoginMenu()
//     {
//         Console.Clear();
//
//         Console.WriteLine("Login");
//         Console.WriteLine("----------------------------------------------");
//
//         Console.Write("Enter email or login: ");
//         var identifier = Console.ReadLine()?.Trim();
//
//         Console.Write("Enter password: ");
//         var password = Console.ReadLine()?.Trim();
//
//         if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
//         {
//             Console.WriteLine("Fields cannot be empty.");
//             Pause();
//             return false;
//         }
//
//         try
//         {
//             AccountManager.LoginByEmail(identifier, password);
//         }
//         catch (UserSearchException)
//         {
//             try
//             {
//                 AccountManager.LoginByUserLogin(identifier, password);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Login failed: {ex.Message}");
//                 Logger.LogException(ex, "LoginByUserLogin");
//                 Pause();
//                 return false;
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Login failed: {ex.Message}");
//             Logger.LogException(ex, "LoginByEmail");
//             Pause();
//             return false;
//         }
//
//         Console.WriteLine($"Welcome, {AccountManager.CurrentUser!.Name}!");
//         Pause();
//         return true;
//     }
//
//     private static void LogoutMenu()
//     {
//         Console.Clear();
//         AccountManager.Logout();
//         Console.WriteLine("Logged out.");
//         Pause();
//     }
//
//     private static void Pause(string message = "Press any key to continue...")
//     {
//         Console.WriteLine(message);
//         Console.ReadKey(true);
//     }
// }
