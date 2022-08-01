using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using NotificationIconSharp;
using System;
using System.Threading.Tasks;

namespace tacttvm
{
    internal class Program
    {
        //v1.0.0.1
        static bool show = false;

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        const string icon_path = @"Assets/translator.ico";

        static bool destroy = false;
        private static Task? task;

        static void Main(string[] args)
        {
            task = new Task(() =>
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            });
            task.Start();

            NotificationManager.Initialize("com.app.test", icon_path, icon_path);
            NotificationManager.NotificationIconSelectedEvent += NotificationManager_NotificationIconSelectedEvent;

            var trayIcon = new NotificationIcon(icon_path);
            TrayIcon_NotificationIconSelected(trayIcon);
            do
            {
                if (App.RefMainWindow != null)
                {
                    Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Close);
                    break;
                }
            } while (true);

            while (!destroy)
            {
               
                trayIcon?.DoMessageLoop(true);
            }

            trayIcon?.Dispose();

            Dispatcher.UIThread.InvokeAsync(() => App.RefMainWindow.Shutdown(1));

            App.DisposeStatic();
        }

        private static void NotificationManager_NotificationIconSelectedEvent(string notificationId)
        {
            Console.WriteLine(notificationId);
        }

        private static void TrayIcon_NotificationIconSelected(NotificationIcon icon)
        {
            if (icon.MenuItems.Count > 0) return;

            var showMainWindow = new NotificationMenuItem("Show");
            showMainWindow.NotificationMenuItemSelected += ShowMainWindow_NotificationMenuItemSelected;


            var setTextMenuItem = new NotificationMenuItem("Help");
            setTextMenuItem.NotificationMenuItemSelected += Help_NotificationMenuItemSelected;

            var disableMenuItem = new NotificationMenuItem("Exit");
            disableMenuItem.NotificationMenuItemSelected += Exit_NotificationMenuItemSelected;

            icon.AddMenuItem(showMainWindow);
            icon.AddMenuItem(disableMenuItem);
            icon.AddMenuItem(setTextMenuItem);
        }

        public static void ShowMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            if (App.RefMainWindow == null)
            {
                throw new ArgumentNullException(nameof(App.RefMainWindow), "App.RefMainWindow is null");
            }
            if (show)
            {
                Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Close);
                menuItem.Text = "Show";
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Show);
                menuItem.Text = "Hide";
            }

            show = !show;
        }

        private static void Exit_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            destroy = true;
        }

        private static void Help_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Code", "https://github.com/Vadimvr/tacttvm");
                messageBoxStandardWindow.Show();
            });
        }
    }
}