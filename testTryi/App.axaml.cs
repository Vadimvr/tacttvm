using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using testTryi.ViewModels;
using testTryi.Views;

namespace testTryi
{
    public partial class App : Application
    {
        public static IClassicDesktopStyleApplicationLifetime RefMainWindow { get; private set; }

        //   public static MainWindow RefMainWindow { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static void Show()
        {
            RefMainWindow.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            RefMainWindow.MainWindow.Show();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                desktop.MainWindow.Closing += (s, e) =>
                {
                    ((Window)s).Hide();
                    e.Cancel = true;
                };

                RefMainWindow = desktop;
                Program.hide = desktop.MainWindow.Hide;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
