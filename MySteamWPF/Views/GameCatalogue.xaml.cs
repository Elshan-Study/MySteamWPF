using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySteamWPF.Core.Data;
using MySteamWPF.Core.Models;

namespace MySteamWPF.Views
{
    public partial class GameCatalogue : UserControl
    {
        private List<Game> _allGames;
        private List<Game> _filteredGames;

        public GameCatalogue()
        {
            InitializeComponent();

            _allGames = Database.Games ?? new List<Game>();
            _filteredGames = _allGames;

            GamesListBox.ItemsSource = _filteredGames;
        }

        private void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            var query = SearchTextBox.Text.Trim().ToLower();

            _filteredGames = _allGames.Where(g =>
                (!string.IsNullOrEmpty(g.Name) && g.Name.ToLower().Contains(query)) ||
                (g.Tags.Any(t => t.ToLower().Contains(query)))
            ).ToList();

            GamesListBox.ItemsSource = _filteredGames;
        }

        private void OnResetClicked(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            _filteredGames = _allGames;
            GamesListBox.ItemsSource = _filteredGames;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSearchClicked(sender, e);
            }
        }

        private void OnGameClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Game selectedGame)
            {
                GamePage.CurrentGame = selectedGame;
                ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GamePage();
            }
        }
    }
}
