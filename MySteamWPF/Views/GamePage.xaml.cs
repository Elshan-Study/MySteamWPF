using System.Windows.Controls;
using MySteamWPF.Core.Models;

namespace MySteamWPF.Views;

public partial class GamePage : UserControl
{
    public static Game? CurrentGame { get; set; }
    public GamePage()
    {
        InitializeComponent();
    }
}