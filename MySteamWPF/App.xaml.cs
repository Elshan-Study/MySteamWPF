using System.Configuration;
using System.Data;
using System.Windows;
using MySteamWPF.Core.Services;

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
        DataManager.LoadAll();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Сохранение всех данных в JSON
        DataManager.SaveAll();

        base.OnExit(e);
    }
}