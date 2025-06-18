using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySteamWPF.Core.Services;
using MySteamWPF.Views;

namespace MySteamWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataManager.LoadAll();
        InitializeComponent();
        MainContentControl.Content = new GameCatalogue();
        DataManager.SaveAll();
    }
}