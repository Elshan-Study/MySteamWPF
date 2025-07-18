using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views;

/// <summary>
/// Interaction logic for displaying detailed game information, including rating, tags, comments, and purchase functionality.
/// </summary>
public partial class GamePage : UserControl
{
    public static Game? CurrentGame { get; set; }
    private bool _sortNewestFirst = true;

    public GamePage()
    {
        InitializeComponent();
        LoadGame();
    }

    /// <summary>
    /// Loads the game data and populates all UI elements.
    /// </summary>
    private void LoadGame()
    {
        var game = CurrentGame;
        if (game == null) return;

        try
        {
            GameName.Text = game.Name;
            GameDescription.Text = game.Description;
            GamePrice.Text = $"Цена: {game.Price:C}";
            GameImage.Source = new BitmapImage(new Uri(PathHelper.ResolvePath(game.ImagePath)));
            AverageRating.Text = $"{game.AverageRating:F2}/5";

            LoadTags(game);
            LoadRatingStars();
            LoadComments(game);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Error loading game page for game {game.Name}");
            MessageBox.Show("Произошла ошибка при загрузке игры.");
        }
    }

    /// <summary>
    /// Loads game tags into the TagPanel.
    /// </summary>
    private void LoadTags(Game game)
    {
        TagPanel.Children.Clear();
        foreach (var tag in game.Tags)
        {
            TagPanel.Children.Add(new Border
            {
                Background = System.Windows.Media.Brushes.LightGray,
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5),
                Margin = new Thickness(3),
                Child = new TextBlock { Text = tag, FontSize = 12 }
            });
        }
    }

    /// <summary>
    /// Displays interactive rating stars and highlights the user's rating if available.
    /// </summary>
    private void LoadRatingStars()
    {
        RatingStars.Items.Clear();
        int userRating = 0;

        if (AccountManager.CurrentUser != null && CurrentGame != null)
            CurrentGame.Ratings.TryGetValue(AccountManager.CurrentUser.Id, out userRating);

        for (int i = 1; i <= 5; i++)
        {
            var star = new TextBlock
            {
                Text = "★",
                FontSize = 20,
                Foreground = i <= userRating ? System.Windows.Media.Brushes.Gold : System.Windows.Media.Brushes.Gray,
                Cursor = Cursors.Hand,
                Tag = i,
                Margin = new Thickness(2)
            };

            star.MouseLeftButtonDown += OnRateClicked;
            star.MouseEnter += OnStarMouseEnter;
            star.MouseLeave += OnStarMouseLeave;

            RatingStars.Items.Add(star);
        }
    }

    /// <summary>
    /// Called when user clicks on a star to rate the game.
    /// </summary>
    private void OnRateClicked(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (CurrentGame == null)
                return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() != true || AccountManager.CurrentUser == null)
                    return;

                LoadRatingStars();
            }

            ((MainWindow)Application.Current.MainWindow).UpdateTopBar();

            if (sender is not TextBlock { Tag: int rating })
                return;

            CurrentGame.Ratings[AccountManager.CurrentUser.Id] = rating;
            AverageRating.Text = $"{CurrentGame.AverageRating:F2}/5";
            DataManager.SaveAll();
            LoadRatingStars();

            Logger.Log($"User {AccountManager.CurrentUser} rated game {CurrentGame.Name} with {rating} stars.");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Error rating game {CurrentGame?.Name} by user {AccountManager.CurrentUser}");
            MessageBox.Show("Ошибка при выставлении оценки.");
        }
    }

    private void OnStarMouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is TextBlock { Tag: int rating })
            HighlightStars(rating);
    }

    private void OnStarMouseLeave(object sender, MouseEventArgs e) => ResetStars();

    private void HighlightStars(int upTo)
    {
        for (var i = 0; i < RatingStars.Items.Count; i++)
            if (RatingStars.Items[i] is TextBlock star)
                star.Foreground = i < upTo ? System.Windows.Media.Brushes.Gold : System.Windows.Media.Brushes.Gray;
    }

    private void ResetStars()
    {
        var userRating = 0;
        if (AccountManager.CurrentUser != null && CurrentGame != null)
            CurrentGame.Ratings.TryGetValue(AccountManager.CurrentUser.Id, out userRating);

        for (var i = 0; i < RatingStars.Items.Count; i++)
            if (RatingStars.Items[i] is TextBlock star)
                star.Foreground = i < userRating ? System.Windows.Media.Brushes.Gold : System.Windows.Media.Brushes.Gray;
    }

    /// <summary>
    /// Loads comments and displays them in the list.
    /// </summary>
      private void LoadComments(Game game)
    {
        try
        {
            var sorted = (_sortNewestFirst
                ? game.Comments.Select(id => DataManager.Comments.FirstOrDefault(c => c.Id == id))
                    .Where(c => c != null).OrderByDescending(c => c!.DatePosted)
                : game.Comments.Select(id => DataManager.Comments.FirstOrDefault(c => c.Id == id))
                    .Where(c => c != null).OrderBy(c => c!.DatePosted)).Cast<Comment>().ToList();

            CommentList.Children.Clear();
            NoCommentsText.Visibility = sorted.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            foreach (var comment in sorted)
            {
                var author = DataManager.Users.FirstOrDefault(u => u.Id == comment.AuthorId);
                var avatarPath = PathHelper.ResolvePath(author?.AvatarPath ?? string.Empty);

                BitmapImage avatarImage;

                if (File.Exists(avatarPath))
                {
                    avatarImage = new BitmapImage(new Uri(avatarPath));
                }
                else
                {
                    var defaultAvatarPath = PathHelper.ResolvePath("Images/Avatars/DefaultAvatar.jpg");
                    avatarImage = new BitmapImage(new Uri(defaultAvatarPath));
                    Logger.Log($"Avatar not found for user {author?.Login ?? "unknown"}, using default avatar.");
                }

                var horizontalPanel = new StackPanel { Orientation = Orientation.Horizontal };
                horizontalPanel.Children.Add(new Image
                {
                    Source = avatarImage,
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5)
                });

                var messageBlock = new StackPanel { Margin = new Thickness(5) };
                messageBlock.Children.Add(new TextBlock
                {
                    Text = author?.Login ?? "Неизвестный пользователь",
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 14
                });
                messageBlock.Children.Add(new TextBlock
                {
                    Text = comment.Message,
                    Foreground = System.Windows.Media.Brushes.WhiteSmoke,
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap
                });
                messageBlock.Children.Add(new TextBlock
                {
                    Text = comment.DatePosted.ToLocalTime().ToString("g"),
                    FontSize = 10,
                    Foreground = System.Windows.Media.Brushes.LightSteelBlue,
                    Margin = new Thickness(0, 4, 0, 0)
                });

                horizontalPanel.Children.Add(messageBlock);

                CommentList.Children.Add(new Border
                {
                    Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(60, 0, 0, 0)),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(5),
                    Child = horizontalPanel
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Error loading comments for {game.Name}");
            MessageBox.Show("Ошибка при загрузке комментариев.");
        }
    }

    private void OnToggleCommentOrder(object sender, RoutedEventArgs e)
    {
        _sortNewestFirst = !_sortNewestFirst;
        SortOrderButton.Content = _sortNewestFirst ? "Сначала старые" : "Сначала новые";
        if (CurrentGame != null)
            LoadComments(CurrentGame);
    }

    private void OnAddComment(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentGame == null)
                return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() != true || AccountManager.CurrentUser == null)
                    return;
            }

            ((MainWindow)Application.Current.MainWindow).UpdateTopBar();

            var text = Microsoft.VisualBasic.Interaction.InputBox("Введите комментарий:", "Оставить комментарий").Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            var comment = new Comment(AccountManager.CurrentUser.Id, text);
            CurrentGame.Comments.Add(comment.Id);
            DataManager.Comments.Add(comment);

            LoadComments(CurrentGame);
            DataManager.SaveAll();

            Logger.Log($"User {AccountManager.CurrentUser} commented on {CurrentGame.Name}: {text}");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Error adding comment by {AccountManager.CurrentUser} on {CurrentGame?.Name}");
            MessageBox.Show("Ошибка при добавлении комментария.");
        }
    }

    private void OnBuyClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentGame == null) return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog() != true || AccountManager.CurrentUser == null)
                    return;
            }

            ((MainWindow)Application.Current.MainWindow).UpdateTopBar();

            var user = AccountManager.CurrentUser;

            if (user.Games.Contains(CurrentGame.Id))
            {
                MessageBox.Show("Вы уже приобрели эту игру.");
                return;
            }

            if (user.Balance < CurrentGame.Price)
            {
                MessageBox.Show("Недостаточно средств для покупки игры.");
                return;
            }

            user.Balance -= CurrentGame.Price;
            user.Games.Add(CurrentGame.Id);
            DataManager.SaveAll();

            MessageBox.Show($"Вы купили {CurrentGame.Name}!");
            Logger.Log($"User {user} purchased game {CurrentGame.Name}");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Error purchasing {CurrentGame?.Name} by {AccountManager.CurrentUser}");
            MessageBox.Show("Ошибка при покупке игры.");
        }
    }
}

