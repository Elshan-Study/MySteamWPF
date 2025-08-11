using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Interaction logic for GameCatalogue.xaml.
/// Represents the main game catalogue page with search and selection capabilities.
/// </summary>
public partial class GameCatalogue : UserControl
{
    private readonly List<Game> _allGames;
    private List<Game> _filteredGames;

    /// <summary>
    /// Initializes the game catalogue and loads all games from the DataManager.
    /// </summary>
    public GameCatalogue()
    {
        InitializeComponent();

        _allGames = DataManager.LoadGames() ?? new List<Game>();
        _filteredGames = _allGames;

        GamesListBox.ItemsSource = _filteredGames;
        Logger.Log($"Game catalogue loaded with {_allGames.Count} games. User: {AccountManager.CurrentUser}");
    }

    /// <summary>
    /// Filters games by search query when search button is clicked.
    /// </summary>
    private void OnSearchClicked(object sender, RoutedEventArgs e)
    {
        var query = SearchTextBox.Text.Trim();

        _filteredGames = _allGames.Where(g =>
            (!string.IsNullOrEmpty(g.Name) && 
             g.Name.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0) ||
            (g.Tags.Any(t => 
                t.Tag.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0))
        ).ToList();

        GamesListBox.ItemsSource = _filteredGames;
        Logger.Log($"Search performed with query: '{query}'. Found {_filteredGames.Count} games. User: {AccountManager.CurrentUser}");
    }

    /// <summary>
    /// Resets the search field and displays all games.
    /// </summary>
    private void OnResetClicked(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        _filteredGames = _allGames;
        GamesListBox.ItemsSource = _filteredGames;
        Logger.Log($"Search reset. Showing all {_allGames.Count} games. User: {AccountManager.CurrentUser}");
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
    /// Triggers search when Enter key is pressed in the search box.
    /// </summary>
    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            OnSearchClicked(sender, e);
        }
    }

    /// <summary>
    /// Navigates to the selected game's page when clicked.
    /// </summary>
    private void OnGameClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Game selectedGame }) return;
        GamePage.CurrentGame = selectedGame;
        ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GamePage();
        Logger.Log($"Navigated to game page: {selectedGame.Name}. User: {AccountManager.CurrentUser}");
    }
}

