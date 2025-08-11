using MySteamWPF.Core.Exceptions;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Utilities;
using System;
using System.Linq;

namespace MySteamWPF.Core.Services
{
    /// <summary>
    /// Static class responsible for user registration, authentication, and logout.
    /// </summary>
    public static class AccountManager
    {
        /// <summary>
        /// Delegate and event for broadcasting system notifications (e.g. successful registration).
        /// </summary>
        public delegate void Action(string message);
        public static event Action? Notify;

        /// <summary>
        /// The currently logged-in user. Null if no user is logged in.
        /// </summary>
        public static User? CurrentUser { get; private set; }

        /// <summary>
        /// Registers a new user and adds them to the database.
        /// </summary>
        /// <param name="login">The login/username</param>
        /// <param name="name">The display name</param>
        /// <param name="email">The user's email address</param>
        /// <param name="password">The raw password</param>
        /// <exception cref="ArgumentException">Thrown if any field is null or empty</exception>
        /// <exception cref="UserExistsException">Thrown if a user with the same login or email already exists</exception>
        public static void Register(string login, string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("All fields must be filled.");

            var users = DataManager.LoadUsers();
            if (users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) || u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new UserExistsException("A user with this login or email already exists.");

            var hashedPassword = PasswordHasher.Hash(password);

            var newUser = new User
            {
                Login = login,
                Name = name,
                Email = email,
                Password = hashedPassword,
            };

            DataManager.AddUser(newUser);

            Notify?.Invoke($"User {login} successfully registered.");
            Logger.Log($"User {login} created");
        }

        /// <summary>
        /// Attempts to log in a user by email and password.
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">Raw password</param>
        /// <exception cref="ArgumentException">Thrown if email or password is null or empty</exception>
        /// <exception cref="UserSearchException">Thrown if no user with the given email is found</exception>
        /// <exception cref="UserPasswordException">Thrown if the password does not match</exception>
        public static void LoginByEmail(string? email, string? password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password are required.");

            var users = DataManager.LoadUsers();
            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                       ?? throw new UserSearchException("User not found.");

            if (!PasswordHasher.Verify(password, user.Password))
                throw new UserPasswordException("Incorrect password.");

            CurrentUser = user;
            Logger.Log($"User {user.Login} logged in");
        }

        /// <summary>
        /// Attempts to log in a user by login name and password.
        /// </summary>
        /// <param name="login">Username/login</param>
        /// <param name="password">Raw password</param>
        /// <exception cref="ArgumentException">Thrown if login or password is null or empty</exception>
        /// <exception cref="UserSearchException">Thrown if no user with the given login is found</exception>
        /// <exception cref="UserPasswordException">Thrown if the password does not match</exception>
        public static void LoginByUserLogin(string? login, string? password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Login and password are required.");

            var users = DataManager.LoadUsers();
            var user = users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase))
                       ?? throw new UserSearchException("User not found.");

            if (!PasswordHasher.Verify(password, user.Password))
                throw new UserPasswordException("Incorrect password.");

            CurrentUser = user;
            Logger.Log($"User {user.Login} logged in");
        }

        /// <summary>
        /// Logs out the current user, if any.
        /// </summary>
        public static void Logout()
        {
            if (CurrentUser != null)
            {
                Logger.Log($"User {CurrentUser.Login} logged out");
                CurrentUser = null;
            }
            else
            {
                Logger.Log("Logout called with no user logged in.");
            }
        }
    }
}