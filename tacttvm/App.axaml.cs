using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IconInTheTaskbar;
using IconInTheTaskbar.Models;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using tacttvm.Models;
using tacttvm.Static;
using tacttvm.ViewModels;
using tacttvm.Views;

namespace tacttvm
{
    public partial class App : Application, IDisposable
    {
        private ContextMenu? _NotifyIconContextMenu = null;
        public MainWindow? _mainWindow = null;
        List<MenuItem> menuItems = new List<MenuItem>();


        private INotifyIcon? notifyIcon = null;
        public INotifyIcon? NotifyIcon { get => notifyIcon; set => notifyIcon = value; }


        //link to the menu in the taskbar to show / hide the window 
        private MenuItem? ShowOrHide;
        private static bool minimized;
        public bool Minimized
        {
            get => minimized; set
            {
                minimized = value;
                if (ShowOrHide != null) ShowOrHide.Header = Minimized ? "Show" : "Hide";
            }
        }
        /// <summary>
        /// Gets or sets the app's notify icon (tray icon).
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            var mainWindowViewModel = new MainWindowViewModel();
            this._mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel,
            };

            // window opening and closing 
            this._mainWindow.Closing += WindowClosing;
            this._mainWindow.Closing += mainWindowViewModel.Closed;
            this._mainWindow.Opened += mainWindowViewModel.Open ;


            Log.Information("App initialization completed!");
        }



        public void WindowClosing(object? s, CancelEventArgs e)
        {
            var temp = (App.Current as App)?.NotifyIcon;
            var window = (s as Window) ?? throw new ArgumentNullException();

            if (temp != null)
            {
                Log.Information("Hiding main window instead of closing it");
                Dispatcher.UIThread.Post(() =>
                {
                    window.Hide();
                });
                temp.Visible = true;
               
            }
            else
            {
                Log.Information("Minimizing main window instead of closing it");
                Dispatcher.UIThread.Post(() =>
                {
                    window.WindowState = WindowState.Minimized;
                });
            }
            e.Cancel = true;
            
            Minimized = true;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Minimized = AppSettings.Instance.StartMinimized;
            // menu in the task bar
            ShowOrHide = new MenuItem()
            {
                Header = Minimized ? "Show" : "Hide",
                Command = ReactiveCommand.Create(RestoreMainWindow)
            };

            menuItems.Add(ShowOrHide);
            menuItems.Add(new MenuItem()
            {
                Header = "Exit",
                Command = ReactiveCommand.Create(Exit)
            });


            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                IconCreate.OnFrameworkInitializationCompletedinApp(
                       desktop,
                   _mainWindow,
                    ref notifyIcon,
                   StaticResources.NotifyIconParameters,
                   this.menuItems,
                   this._NotifyIconContextMenu,
                   false,
                   RestoreMainWindow);
                if (desktop != null)
                {
                    desktop.MainWindow.Activated += HedeWnenStart;
                }
            }

            base.OnFrameworkInitializationCompleted();
        }


        // needed for Linux. window doesn't open when minimized
        public void HedeWnenStart(object? s, EventArgs a)
        {
            var window = (s as Window) ?? throw new ArgumentNullException();
            if (window != null)
            {
                if (AppSettings.Instance.StartMinimized)
                {
                    window.Hide();
                }
                window.Activated -= HedeWnenStart;
            }

        }

        // The method that is called when clicking on a field in the taskbar
        public void RestoreMainWindow()
        {
            Log.Information("Restoring main window");

            Window? mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .MainWindow ?? _mainWindow;

            if (mainWindow == null)
            {
                Log.Information($"Failed to display the window because {(Application.Current is null ? "Application.Current is null" : "_mainWindow is null")}");
                return;
            }
            if (Minimized)
            {
                mainWindow.Show();
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.BringIntoView();
                mainWindow.ActivateWorkaround(); // Extension method hack because of https://github.com/AvaloniaUI/Avalonia/issues/2975
                mainWindow.Focus();

                // Again, ugly hack because of https://github.com/AvaloniaUI/Avalonia/issues/2994
                mainWindow.Width += 0.1;
                mainWindow.Width -= 0.1;
                Minimized = false;
            }
            else
            {
                // Changing the title only works in Windows 
                foreach (var item in menuItems)
                {
                    if (item.Header.ToString() == "Hide")
                    {
                        item.Header = "Show";
                        break;
                    }
                }
                mainWindow.Close();
                Minimized = true;
            }
        }

        /// <summary>
        /// Exits the app by calling <c>Shutdown()</c> on the <c>IClassicDesktopStyleApplicationLifetime</c>.
        /// </summary>
        public void Exit()
        {
            (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .Shutdown(0);
        }

        public static IClassicDesktopStyleApplicationLifetime? RefMainWindow { get; private set; }
       
        public static  MainWindowViewModel? mainWindowViewModel { get; private set; }

        public static App? app; 

        //   public static MainWindow RefMainWindow { get; private set; }

     

       static public void DisposeStatic()
        {
            app?.Dispose();
        }

        public void Dispose()
        {
           GC.SuppressFinalize(this);
        }
    }

}
