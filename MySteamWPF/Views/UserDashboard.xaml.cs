using System.Windows.Controls;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views
{
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
            _visibleGames = GetUserGames(_user.Games);
            _hiddenGames = GetUserGames(_user.HiddenGames);

            LoadUserInfo();
            ShowVisibleGames();
        }

        private List<Game> GetUserGames(List<string> gameIds)
        {
            if (DataManager.Games != null)
                return DataManager.Games
                    .Where(g => gameIds.Contains(g.Id))
                    .ToList();
            return [];
        }

        private void LoadUserInfo()
        {
            NameTextBlock.Text = _user.Name;
            LoginTextBlock.Text = _user.Login;
            EmailTextBlock.Text = _user.Email;
            BalanceTextBlock.Text = $"{_user.Balance:C}";

            // Аватар
            string fullPath = Path.GetFullPath(_user.AvatarPath);
            if (!string.IsNullOrEmpty(_user.AvatarPath) && File.Exists(fullPath))
                AvatarPath.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
            else
                AvatarPath.Source = new BitmapImage(new Uri("Images/Avatars/DefaultAvatar.jpg", UriKind.Relative));
        }

        private void ShowVisibleGames()
        {
            Tag = "ShowVisible";
            _showingHidden = false;
            GamesList.ItemsSource = null;
            GamesList.ItemsSource = _visibleGames;
            NoGamesTextBlock.Visibility = _visibleGames.Count != 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ShowHiddenGames()
        {
            Tag = "ShowHidden";
            _showingHidden = true;
            GamesList.ItemsSource = null;
            GamesList.ItemsSource = _hiddenGames;
            NoGamesTextBlock.Visibility = _hiddenGames.Count != 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnSearch(object sender, RoutedEventArgs e)
        {
            var query = SearchTextBox.Text.Trim().ToLower();
            var source = _showingHidden ? _hiddenGames : _visibleGames;

            var filtered = source
                .Where(g => g.Name.ToLower().Contains(query) ||
                            g.Tags.Any(t => t.ToLower().Contains(query)))
                .ToList();

            GamesList.ItemsSource = filtered;
            NoGamesTextBlock.Visibility = filtered.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnToggleHiddenGames(object sender, RoutedEventArgs e)
        {
            if (_showingHidden)
                ShowVisibleGames();
            else
                ShowHiddenGames();
        }

        private void OnTopUpClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new TopUpDialog();
            if (dialog.ShowDialog() == true && dialog.Amount > 0)
            {
                _user.Balance += dialog.Amount;
                BalanceTextBlock.Text = $"{_user.Balance:C}";
            }
        }

        private void OnChangeAvatarClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dlg.ShowDialog() == true)
            {
                string extension = Path.GetExtension(dlg.FileName);
                string uniqueFileName = $"{Guid.NewGuid()}{extension}";

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string avatarsFolder = Path.Combine(appDirectory, "Images", "Avatars");
                Directory.CreateDirectory(avatarsFolder);

                string targetPath = Path.Combine(avatarsFolder, uniqueFileName);
                File.Copy(dlg.FileName, targetPath, overwrite: true);

                _user.AvatarPath = Path.Combine("Images", "Avatars", uniqueFileName);
                AvatarPath.Source = new BitmapImage(new Uri(Path.GetFullPath(_user.AvatarPath), UriKind.Absolute));
            }
        }

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
        }

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

            if (DataManager.Users.Any(u => u.Login.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Этот логин уже используется.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _user.Login = input;
            LoginTextBlock.Text = _user.Login;
            MessageBox.Show("Логин успешно обновлён.", "Успех", MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }
        
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
            
            if (DataManager.Users.Any(u => u.Email.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Этот email уже используется.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _user.Email = input;
            EmailTextBlock.Text = _user.Email;
            MessageBox.Show("Email успешно обновлён.", "Успех", MessageBoxButton.OK, 
                MessageBoxImage.Information);
        }

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
        }
        
        private void OnGameClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: Game selectedGame })
            {
                GamePage.CurrentGame = selectedGame;
                ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GamePage();
            }
        }
        
        private void OnHideUnhideClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: Game game })
            {
                if (_showingHidden)
                {
                    _user.HiddenGames.Remove(game.Id);
                    if (!_user.Games.Contains(game.Id))
                        _user.Games.Add(game.Id);
                }
                else
                {
                    _user.Games.Remove(game.Id);
                    if (!_user.HiddenGames.Contains(game.Id))
                        _user.HiddenGames.Add(game.Id);
                }
                
                _visibleGames = GetUserGames(_user.Games).ToList();
                _hiddenGames = GetUserGames(_user.HiddenGames).ToList();
                
                GamesList.ItemsSource = null;
                if (_showingHidden)
                    ShowHiddenGames();
                else
                    ShowVisibleGames();
            }
        }
        
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnSearch(sender, e);
        }
    }


}
