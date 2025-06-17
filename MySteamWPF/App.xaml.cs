using System.Configuration;
using System.Data;
using System.Windows;
using MySteamWPF.Core.Data;

namespace MySteamWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Загрузка всех данных из JSON
        Database.LoadAll();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Сохранение всех данных в JSON
        Database.SaveAll();

        base.OnExit(e);
    }
}