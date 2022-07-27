using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Serilog;
using System.Linq;
using tacttvm.ViewModels;
using tacttvm.Views;

namespace tacttvm
{
    public partial class App : Application
    {

        private ContextMenu _NotifyIconContextMenu = null;
        // public LocalizationExtension Localization { get; }
        private MainWindow _mainWindow = null;

        public static Window? ActivedWindow()
        {
            if (Current != null && (Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop))
            {
                return desktop.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);
            }

            return null;
        }

        public static Window? FocusedWindow()
        {
            if (Current != null && (Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop))
            {
                return desktop.Windows.Cast<Window>().FirstOrDefault(w => w.IsFocused);
            }

            return null;
        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            this._mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            Log.Information("App initialization completed!");
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                }
                //_quickPassManager = QuickPassManager.Instance;
                            //desktop.MainWindow = _mainWindow;
                base.OnFrameworkInitializationCompleted();
            }
        }


        /// <summary>
        /// Restores the app's main window by setting its <c>WindowState</c> to
        /// <c>WindowState.Normal</c> and showing the window.
        /// </summary>

    
}
