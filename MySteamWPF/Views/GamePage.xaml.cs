using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using MySteamWPF.Core.Utilities;

namespace MySteamWPF.Views
{
    public partial class GamePage : UserControl
    {
        public static Game? CurrentGame { get; set; }
        private bool _sortNewestFirst = true;

        public GamePage()
        {
            InitializeComponent();
            LoadGame();
        }

        private void LoadGame()
        {
            var game = CurrentGame;
            if (game == null) return;

            GameName.Text = game.Name;
            GameDescription.Text = game.Description;
            GamePrice.Text = $"Цена: {game.Price:C}";
            GameImage.Source = new BitmapImage(new Uri(PathHelper.ResolvePath(game.ImagePath)));
            AverageRating.Text = $"{game.AverageRating:F2}/5";

            LoadTags(game);
            LoadRatingStars();
            LoadComments(game);
        }

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

        private void LoadRatingStars()
        {
            RatingStars.Items.Clear();

            int userRating = 0;
            if (AccountManager.CurrentUser != null && CurrentGame != null)
            {
                CurrentGame.Ratings.TryGetValue(AccountManager.CurrentUser.Id, out userRating);
            }

            for (int i = 1; i <= 5; i++)
            {
                var star = new TextBlock
                {
                    Text = "★",
                    FontSize = 20,
                    Foreground = i <= userRating ? System.Windows.Media.Brushes.Gold : 
                        System.Windows.Media.Brushes.Gray,
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

        private void OnRateClicked(object sender, MouseButtonEventArgs e)
        {
            if (CurrentGame == null)
                return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                bool? result = loginWindow.ShowDialog();
                if (result != true || AccountManager.CurrentUser == null)
                    return;
                
                LoadRatingStars();
            }
            
            ((MainWindow)Application.Current.MainWindow).UpdateTopBar();

            if (sender is not TextBlock star || star.Tag is not int rating) return;

            CurrentGame.Ratings[AccountManager.CurrentUser.Id] = rating;
            AverageRating.Text = $"{CurrentGame.AverageRating:F2}/5";
            DataManager.SaveAll();
            LoadRatingStars();
        }

        
        private void OnStarMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock { Tag: int rating }) return;
            HighlightStars(rating);
        }

        private void OnStarMouseLeave(object sender, MouseEventArgs e)
        {
            ResetStars();
        }

        private void HighlightStars(int upTo)
        {
            for (int i = 0; i < RatingStars.Items.Count; i++)
            {
                if (RatingStars.Items[i] is TextBlock star)
                {
                    star.Foreground = i < upTo
                        ? System.Windows.Media.Brushes.Gold
                        : System.Windows.Media.Brushes.Gray;
                }
            }
        }

        private void ResetStars()
        {
            var userRating = 0;
            if (AccountManager.CurrentUser != null && CurrentGame != null)
            {
                CurrentGame.Ratings.TryGetValue(AccountManager.CurrentUser.Id, out userRating);
            }

            for (int i = 0; i < RatingStars.Items.Count; i++)
            {
                if (RatingStars.Items[i] is TextBlock star)
                {
                    star.Foreground = i < userRating
                        ? System.Windows.Media.Brushes.Gold
                        : System.Windows.Media.Brushes.Gray;
                }
            }
        }

        private void LoadComments(Game game)
        {
            var commentIds = game.Comments;
            var comments = commentIds
                .Select(id => DataManager.Comments.FirstOrDefault(c => c.Id == id))
                .Where(c => c != null)
                .Cast<Comment>()
                .ToList();

            var sorted = _sortNewestFirst
                ? comments.OrderByDescending(c => c.DatePosted)
                : comments.OrderBy(c => c.DatePosted);

            CommentList.Children.Clear();

            var sortedList = sorted.ToList();
            NoCommentsText.Visibility = sortedList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            foreach (var comment in sortedList)
            {
                var author = DataManager.Users.FirstOrDefault(u => u.Id == comment.AuthorId);
                var avatarPath = PathHelper.ResolvePath(author?.AvatarPath ?? string.Empty);
                
                var horizontalPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                horizontalPanel.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(avatarPath)),
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5)
                });

                var messageBlock = new StackPanel
                {
                    Margin = new Thickness(5)
                };

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
                
                var borderedPanel = new Border
                {
                    Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromArgb(60, 0, 0, 0)
                    ),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(5),
                    Child = horizontalPanel
                };

                CommentList.Children.Add(borderedPanel);
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
            if (CurrentGame == null)
                return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();
                if (result != true || AccountManager.CurrentUser == null)
                    return;
            }
            
            ((MainWindow)Application.Current.MainWindow).UpdateTopBar();
            
            var user = AccountManager.CurrentUser;
            var game = CurrentGame;

            var text = Microsoft.VisualBasic.Interaction.InputBox("Введите комментарий:", 
                "Оставить комментарий").Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            var comment = new Comment(user.Id, text);
            game.Comments.Add(comment.Id);
            DataManager.Comments.Add(comment);

            LoadComments(game);
            DataManager.SaveAll();
        }

        private void OnBuyClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentGame == null)
                return;

            if (AccountManager.CurrentUser == null)
            {
                var loginWindow = new LoginWindow();
                var result = loginWindow.ShowDialog();
                if (result != true || AccountManager.CurrentUser == null)
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
        }

    }
}
