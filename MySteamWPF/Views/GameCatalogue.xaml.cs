using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
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

        _allGames = DataManager.LoadGames();
        _filteredGames = _allGames;

        GamesListBox.ItemsSource = _filteredGames;
        
        Dispatcher.BeginInvoke(new Action(UpdateButtonsVisibility), DispatcherPriority.Loaded);
        
        Logger.Log($"Game catalogue loaded with {_allGames.Count} games. User: {AccountManager.CurrentUser}");
    }

    /// <summary>
    /// Filters games by search query when search button is clicked.
    /// </summary>
    private void OnSearchClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var query = SearchTextBox.Text.Trim();

            _filteredGames = _allGames.Where(g =>
                (!string.IsNullOrEmpty(g.Name) &&
                 g.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)) ||
                (g.GameTags.Any(t =>
                    t.Tag.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)))
            ).ToList();

            GamesListBox.ItemsSource = _filteredGames;
            Dispatcher.BeginInvoke(new Action(UpdateButtonsVisibility), DispatcherPriority.Loaded);
            Logger.Log($"Search performed with query: '{query}'. Found {_filteredGames.Count} games. User: {AccountManager.CurrentUser}");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Error during search.");
            MessageBox.Show("Ошибка при поиске игр.");
        }
    }

    /// <summary>
    /// Resets the search field and displays all games.
    /// </summary>
    private void OnResetClicked(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        _filteredGames = _allGames;
        GamesListBox.ItemsSource = _filteredGames;
        Dispatcher.BeginInvoke(new Action(UpdateButtonsVisibility), DispatcherPriority.Loaded);
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
        try
        {
            if (sender is not FrameworkElement { DataContext: Game selectedGame })
            {
                Logger.Log("Game click event triggered but DataContext is not Game.");
                return;
            }

            GamePage.CurrentGame = selectedGame;
            ((MainWindow)Application.Current.MainWindow).MainContentControl.Content = new GamePage();
            Logger.Log($"Navigated to game page: {selectedGame.Name}. User: {AccountManager.CurrentUser}");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Error while navigating to game page.");
            MessageBox.Show("Ошибка при открытии страницы игры.");
        }
    }
    
    /// <summary>
    /// Delete selected game when clicked.
    /// </summary>
    private void OnDeleteGameFromCatalogueClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: Game game }) return;

        if (MessageBox.Show($"Вы уверены, что хотите удалить игру '{game.Name}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        try
        {
            DataManager.DeleteGame(game.Id);
            Logger.Log($"Game '{game.Name}' deleted by {AccountManager.CurrentUser?.Login}");
            
            _allGames.Remove(game);
            _filteredGames.Remove(game);

            GamesListBox.ItemsSource = null;
            GamesListBox.ItemsSource = _filteredGames;
            
            var userGame = AccountManager.CurrentUser?.UserGames
                ?.FirstOrDefault(ug => ug.GameId == game.Id);

            if (userGame != null)
            {
                AccountManager.CurrentUser?.UserGames?.Remove(userGame);
            }

            Dispatcher.BeginInvoke(new Action(UpdateButtonsVisibility), DispatcherPriority.Loaded);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, "Error deleting game from catalogue");
            MessageBox.Show("Ошибка при удалении игры.");
        }
    }
    
    private void UpdateButtonsVisibility()
    {
        foreach (var item in GamesListBox.Items)
        {
            var container = (ContentPresenter)GamesListBox.ItemContainerGenerator.ContainerFromItem(item);
            if (container == null) continue;
            var deleteButton = FindDeleteButton(container);
            if (deleteButton != null)
            {
                deleteButton.Visibility = AccountManager.CurrentUser?.IsGaben == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        
        AddGameButton.Visibility = AccountManager.CurrentUser?.IsGaben == true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private Button? FindDeleteButton(DependencyObject parent)
    {
        if (parent is Button { Name: "DeleteButton" } btn)
            return btn;

        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            var result = FindDeleteButton(child);
            if (result != null)
                return result;
        }
        return null;
    }
    
    private void OnAddGameClicked(object sender, RoutedEventArgs e)
    {
        var addGameWindow = new AddGameWindow();
        if (addGameWindow.ShowDialog() != true) return;
        var newGame = addGameWindow.CreatedGame;
        if (newGame == null) return;
        _allGames.Add(newGame);
        _filteredGames.Add(newGame);
        GamesListBox.ItemsSource = null;
        GamesListBox.ItemsSource = _filteredGames;
        Dispatcher.BeginInvoke(new Action(UpdateButtonsVisibility), DispatcherPriority.Loaded);
        Logger.Log($"New game '{newGame.Name}' added by {AccountManager.CurrentUser?.Login}");
    }


}