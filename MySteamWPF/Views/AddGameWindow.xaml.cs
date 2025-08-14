using Microsoft.Win32;
using MySteamWPF.Core.Models;
using MySteamWPF.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MySteamWPF.Views
{
    public partial class AddGameWindow : Window
    {
        public Game? CreatedGame { get; private set; }
        private string? _selectedImagePath;
        private readonly ObservableCollection<Tag> _allTags = new();
        private readonly ObservableCollection<Tag> _selectedTags = new();
        public AddGameWindow()
        {
            InitializeComponent();
            foreach (var tag in DataManager.LoadTags())
                _allTags.Add(tag);
            
            AllTagsListBox.ItemsSource = _allTags;
            AllTagsListBox.DisplayMemberPath = "Name";

            TagsListBox.ItemsSource = _selectedTags;
            TagsListBox.DisplayMemberPath = "Name";
        }

        private void OnSelectImageClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Изображения (*.png;*.jpg)|*.png;*.jpg" };
            if (dialog.ShowDialog() != true) return;
            _selectedImagePath = dialog.FileName;
            CoverImage.Source = new BitmapImage(new Uri(_selectedImagePath));
        }

        private void OnAddTagClicked(object sender, RoutedEventArgs e)
        {
            var newTagName = NewTagTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newTagName)) return;
            
            if (_selectedTags.Any(t => t.Name.Equals(newTagName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Такой тег уже выбран.");
                return;
            }

            var tag = _allTags.FirstOrDefault(t => t.Name.Equals(newTagName, StringComparison.OrdinalIgnoreCase));
            if (tag == null)
            {
                tag = new Tag { Name = newTagName };
                _allTags.Add(tag);
            }

            _selectedTags.Add(tag); 
            NewTagTextBox.Clear();
        }

        private void OnAddClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) { MessageBox.Show("Введите название игры."); return; }
            var allGames = DataManager.LoadGames();
            if (allGames.Any(g => g.Name.Equals(NameTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Игра с таким названием уже существует.");
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text, out var price) || price < 0) { MessageBox.Show("Введите корректную цену."); return; }
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text)) { MessageBox.Show("Введите описание игры."); return; }

            var newGame = new Game
            {
                Name = NameTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                Price = price,
                ImagePath = _selectedImagePath ?? string.Empty
            };
            
            try
            {
                DataManager.AddGame(newGame, _selectedTags.ToList());
                CreatedGame = newGame;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении игры: {ex.Message}");
            }
        }
        
        private void OnTagDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (TagsListBox.SelectedItem is not Tag selectedTag) return;
            _selectedTags.Remove(selectedTag);
        }
        private void OnExistingTagDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (AllTagsListBox.SelectedItem is not Tag tag) return;
            if (_selectedTags.Any(t => t.Id == tag.Id)) return;
            _selectedTags.Add(tag);
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}