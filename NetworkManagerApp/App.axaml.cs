using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NetworkManagerApp.Services;
using NetworkManagerApp.ViewModels;
using NetworkManagerApp.Views;

namespace NetworkManagerApp
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(new NetworkConfigurationService()),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}