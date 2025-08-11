using System.Windows.Controls;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// User dashboard page allowing to view and manage visible and hidden games,
/// user information editing, avatar change, balance top-up, and password updates.
/// </summary>
public partial class UserDashboard : UserControl
{
    private readonly User _user;
    private List<Game> _visibleGames;
    private List<Game> _hiddenGames;
    private bool _showingHidden;

    public UserDashboard()
    {
        InitializeComponent();
        Tag = "ShowVisible";

        _user = AccountManager.CurrentUser!;
        _visibleGames = GetUserGames(_user.UserGames);
        _hiddenGames = GetUserGames(_user.HiddenGames);

        LoadUserInfo();
        ShowVisibleGames();

        Logger.Log($"UserDashboard initialized for user {_user.Login}");
    }

    /// <summary>
    /// Retrieves the list of Game objects matching the given game IDs.
    /// </summary>
    private List<Game> GetUserGames(List<UserGame> userGames)
    {
        if (DataManager.LoadGames() == null)
        {
            Logger.Log($"DataManager.Games is null when retrieving user games for user {_user.Login}", "Warning");
            return new List<Game>();
        }

        var gameIds = userGames.Select(ug => ug.GameId).ToList();

        return DataManager.LoadGames() 
            .Where(g => gameIds.Contains(g.Id))
            .ToList();
    }

    /// <summary>
    /// Loads user information into the UI elements.
    /// </summary>
    private void LoadUserInfo()
    {
        NameTextBlock.Text = _user.Name;
        LoginTextBlock.Text = _user.Login;
        EmailTextBlock.Text = _user.Email;
        BalanceTextBlock.Text = $"{_user.Balance:C}";

        try
        {
            if (!string.IsNullOrEmpty(_user.AvatarPath))
            {
                var fullPath = PathHelper.ResolvePath(_user.AvatarPath);
                if (File.Exists(fullPath))
                {
                    AvatarPath.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                    return;
                }
            }
            
            var defaultAvatarPath = PathHelper.ResolvePath("Images/Avatars/DefaultAvatar.jpg");
            AvatarPath.Source = File.Exists(defaultAvatarPath) ? new BitmapImage(new Uri(defaultAvatarPath, UriKind.Absolute)) : null;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Failed to load avatar for user {_user.Login}");
            AvatarPath.Source = null;
        }
    }

    /// <summary>
    /// Shows the list of visible games.
    /// </summary>
    private void ShowVisibleGames()
    {
        Tag = "ShowVisible";
        _showingHidden = false;
        GamesList.ItemsSource = null;
        GamesList.ItemsSource = _visibleGames;
        NoGamesTextBlock.Visibility = _visibleGames.Count != 0 ? Visibility.Collapsed : Visibility.Visible;

        Logger.Log($"User {_user.Login} switched to visible games view. Count: {_visibleGames.Count}");
    }

    /// <summary>
    /// Shows the list of hidden games.
    /// </summary>
    private void ShowHiddenGames()
    {
        Tag = "ShowHidden";
        _showingHidden = true;
        GamesList.ItemsSource = null;
        GamesList.ItemsSource = _hiddenGames;
        NoGamesTextBlock.Visibility = _hiddenGames.Count != 0 ? Visibility.Collapsed : Visibility.Visible;

        Logger.Log($"User {_user.Login} switched to hidden games view. Count: {_hiddenGames.Count}");
    }

    /// <summary>
    /// Searches games based on the current search query and active game list.
    /// </summary>
    private void OnSearch(object sender, RoutedEventArgs e)
    {
        var query = SearchTextBox.Text.Trim().ToLower();
        var source = _showingHidden ? _hiddenGames : _visibleGames;

        var filtered = source
            .Where(g => g.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                        g.Tags.Any(t => t.Tag.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
            .ToList();

        GamesList.ItemsSource = filtered;
        NoGamesTextBlock.Visibility = filtered.Count != 0 ? Visibility.Collapsed : Visibility.Visible;

        Logger.Log($"User {_user.Login} performed search with query '{query}' on " +
                   $"{(_showingHidden ? "hidden" : "visible")} games, found {filtered.Count} results.");
    }

    /// <summary>
    /// Toggles between showing hidden and visible games.
    /// </summary>
    private void OnToggleHiddenGames(object sender, RoutedEventArgs e)
    {
        if (_showingHidden)
            ShowVisibleGames();
        else
            ShowHiddenGames();
    }

    /// <summary>
    /// Opens the top-up dialog to increase user balance.
    /// </summary>
    private void OnTopUpClicked(object sender, RoutedEventArgs e)
    {
        var dialog = new TopUpDialog();
        if (dialog.ShowDialog() != true || dialog.Amount <= 0) return;
        _user.Balance += dialog.Amount;
        BalanceTextBlock.Text = $"{_user.Balance:C}";
        DataManager.UpdateUser(_user);

        Logger.Log($"User {_user.Login} topped up balance by {dialog.Amount:C}. New balance: {_user.Balance:C}");
    }

    /// <summary>
    /// Allows the user to change their avatar by selecting an image file.
    /// </summary>
    private void OnChangeAvatarClicked(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
        };

        if (dlg.ShowDialog() != true) return;
        try
        {
            var extension = Path.GetExtension(dlg.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var avatarsFolder = Path.Combine(appDirectory, "Images", "Avatars");
            Directory.CreateDirectory(avatarsFolder);

            var targetPath = Path.Combine(avatarsFolder, uniqueFileName);
            File.Copy(dlg.FileName, targetPath, overwrite: true);

            _user.AvatarPath = Path.Combine("Images", "Avatars", uniqueFileName);
            AvatarPath.Source = new BitmapImage(new Uri(Path.GetFullPath(_user.AvatarPath), UriKind.Absolute));

            DataManager.UpdateUser(_user);
            Logger.Log($"User {_user.Login} changed avatar to {uniqueFileName}");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Failed to change avatar for user {_user.Login}");
            MessageBox.Show("Не удалось изменить аватар. Попробуйте снова.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Changes user's name after validation.
    /// </summary>
    private void OnChangeNameClicked(object sender, RoutedEventArgs e)
    {
        var input = Microsoft.VisualBasic.Interaction.InputBox("Введите новое имя:",
            "Изменить имя", _user.Name);

        if (string.IsNullOrWhiteSpace(input) || input == _user.Name)
            return;

        input = input.Trim();

        if (!Validator.IsValidName(input))
        {
            MessageBox.Show("Имя должно содержать минимум 2 символа.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _user.Name = input;
        NameTextBlock.Text = _user.Name;
        MessageBox.Show("Имя успешно обновлено.", "Успех", MessageBoxButton.OK,
            MessageBoxImage.Information);
        DataManager.UpdateUser(_user);

        Logger.Log($"User {_user.Login} changed name to '{_user.Name}'");
    }

    /// <summary>
    /// Changes user's login after validation and uniqueness check.
    /// </summary>
    private void OnChangeLoginClicked(object sender, RoutedEventArgs e)
    {
        var input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый логин:",
            "Изменить логин", _user.Login);

        if (string.IsNullOrWhiteSpace(input) || input == _user.Login)
            return;

        input = input.Trim();

        if (!Validator.IsValidLogin(input))
        {
            MessageBox.Show("Логин должен быть от 3 символов, содержать буквы, цифры, _ или -.",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (DataManager.LoadUsers().Any(u => u.Login.Equals(input, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Этот логин уже используется.", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        _user.Login = input;
        LoginTextBlock.Text = _user.Login;
        MessageBox.Show("Логин успешно обновлён.", "Успех", MessageBoxButton.OK,
            MessageBoxImage.Information);
        DataManager.UpdateUser(_user);

        Logger.Log($"User changed login to '{_user.Login}'");
    }

    /// <summary>
    /// Changes user's email after validation and uniqueness check.
    /// </summary>
    private void OnChangeEmailClicked(object sender, RoutedEventArgs e)
    {
        var input = Microsoft.VisualBasic.Interaction.InputBox("Введите новый email:",
            "Изменить email", _user.Email);

        if (string.IsNullOrWhiteSpace(input) || input == _user.Email)
            return;

        input = input.Trim();

        if (!Validator.IsValidEmail(input))
        {
            MessageBox.Show("Введите корректный email (пример: user@example.com).",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (DataManager.LoadUsers().Any(u => u.Email.Equals(input, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Этот email уже используется.", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        _user.Email = input;
        EmailTextBlock.Text = _user.Email;
        MessageBox.Show("Email успешно обновлён.", "Успех", MessageBoxButton.OK,
            MessageBoxImage.Information);
        DataManager.UpdateUser(_user);

        Logger.Log($"User {_user.Login} changed email to '{_user.Email}'");
    }

    /// <summary>
    /// Changes user's password after validating old password and new password rules.
    /// </summary>
    private void OnChangePasswordClicked(object sender, RoutedEventArgs e)
    {
        var passwordWindow = new ChangePasswordWindow
        {
            Owner = Window.GetWindow(this)
        };

        passwordWindow.ShowDialog();

        if (!passwordWindow.IsConfirmed)
            return;

        var oldPassword = passwordWindow.OldPassword;
        var newPassword = passwordWindow.NewPassword;

        if (string.IsNullOrEmpty(oldPassword) || !PasswordHasher.Verify(oldPassword, _user.Password))
        {
            MessageBox.Show("Неверный старый пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (!Validator.IsValidPassword(newPassword))
        {
            MessageBox.Show("Пароль должен быть от 8 символов, содержать буквы и цифры.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        _user.Password = PasswordHasher.Hash(newPassword);
        MessageBox.Show("Пароль успешно изменён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        DataManager.UpdateUser(_user);

        Logger.Log($"User {_user.Login} changed password.");
    }

    /// <summary>
    /// Opens the game page for the clicked game.
    /// </summary>
    private void OnGameClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Game selectedGame }) return;
        GamePage.CurrentGame = selectedGame;
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GamePage();

        Logger.Log($"User {_user.Login} opened game page for '{selectedGame.Name}'.");
    }

    /// <summary>
    /// Handles the hide/unhide game button click: moves games between visible and hidden lists and updates UI.
    /// </summary>
    private void OnHideUnhideClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: Game game }) return;

        if (_showingHidden)
        {
            var hiddenGame = _user.HiddenGames.FirstOrDefault(ug => ug.GameId == game.Id);
            if (hiddenGame != null)
                _user.HiddenGames.Remove(hiddenGame);

            if (_user.UserGames.All(ug => ug.GameId != game.Id))
                _user.UserGames.Add(new UserGame { UserId = _user.Id, GameId = game.Id });
            DataManager.UpdateUser(_user);

            Logger.Log($"User {_user.Login} unhid game '{game.Name}'.");
        }
        else
        {
            var visibleGame = _user.UserGames.FirstOrDefault(ug => ug.GameId == game.Id);
            if (visibleGame != null)
                _user.UserGames.Remove(visibleGame);

            if (_user.HiddenGames.All(ug => ug.GameId != game.Id))
                _user.HiddenGames.Add(new UserGame { UserId = _user.Id, GameId = game.Id });
            DataManager.UpdateUser(_user);

            Logger.Log($"User {_user.Login} hid game '{game.Name}'.");
        }

        _visibleGames = GetUserGames(_user.UserGames).ToList();
        _hiddenGames = GetUserGames(_user.HiddenGames).ToList();

        GamesList.ItemsSource = null;

        if (_showingHidden)
            ShowHiddenGames();
        else
            ShowVisibleGames();
    }
    
    /// <summary>
        /// Updates visibility of the placeholder text inside the search box.
        /// </summary>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

    /// <summary>
    /// Allows searching on Enter key press.
    /// </summary>
    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            OnSearch(sender, e);
    }
}

