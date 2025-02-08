using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using System;
using System.Security.Principal;
using Avalonia.Controls.ApplicationLifetimes;

namespace NetworkManagerApp
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var appBuilder = BuildAvaloniaApp();
            var appLifetime = new ClassicDesktopStyleApplicationLifetime();
            appBuilder.SetupWithLifetime(appLifetime);

            if (!IsRunningAsAdministrator())
            {
                ShowAdminWarning(appLifetime);
            }

            appLifetime.Start(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static bool IsRunningAsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static void ShowAdminWarning(ClassicDesktopStyleApplicationLifetime appLifetime)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var window = new Window
                {
                    Title = "Aviso",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var textBlock = new TextBlock
                {
                    Text = "Por favor, execute o aplicativo como Administrador!",
                    Margin = new Avalonia.Thickness(20),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                };

                var button = new Button
                {
                    Content = "OK",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Margin = new Avalonia.Thickness(0, 10, 0, 0)
                };

                button.Click += (_, _) =>
                {
                    window.Close();
                    appLifetime.Shutdown();
                };

                var stackPanel = new StackPanel();
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(button);
                window.Content = stackPanel;

                await window.ShowDialog(appLifetime.MainWindow);
            });
        }
    }
}
