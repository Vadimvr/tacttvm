using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using NotificationIconSharp;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using testTryi.ViewModels;
using testTryi.Views;

namespace testTryi
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        static bool show = false;

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        const string icon_path = @"Assets/1.ico";

        static bool destroy = false;
        private static Task? task;

        static void Main(string[] args)
        {
            CancellationToken token = cancelTokenSource.Token;
            task = new Task(() =>
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }, token);
            task.Start();




            NotificationManager.Initialize("com.app.test", icon_path, icon_path);
            // NotificationManager.SendNotification("My New Notification", "Isn't This Handy", "ActionId", icon_path);
            NotificationManager.NotificationIconSelectedEvent += NotificationManager_NotificationIconSelectedEvent;

            var trayIcon = new NotificationIcon(icon_path);
            TrayIcon_NotificationIconSelected(trayIcon);
            //trayIcon.NotificationIconSelected += TrayIcon_NotificationIconSelected;
            do
            {
                if (App.RefMainWindow != null)
                {
                    Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Hide);
                    App.RefMainWindow.MainWindow.Closing += Closing;
                    break;
                }
            } while (true);

            while (true)
            {
                trayIcon?.DoMessageLoop(true);

                if (destroy)
                {
                    trayIcon?.Dispose();
                    trayIcon = null;
                    break;
                }
            }
        }

        static public void Closing(object? w, CancelEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() => ((Window?)w)?.Hide());
            e.Cancel = true;
        }

        private static void NotificationManager_NotificationIconSelectedEvent(string notificationId)
        {
            Console.WriteLine(notificationId);
        }

        private static void TrayIcon_NotificationIconSelected(NotificationIcon icon)
        {
            if (icon.MenuItems.Count > 0) return;


            //var hideMainWindow = new NotificationMenuItem("hide");
            //hideMainWindow.NotificationMenuItemSelected += HideMainWindow_NotificationMenuItemSelected;

            var showMainWindow = new NotificationMenuItem("Show");
            showMainWindow.NotificationMenuItemSelected += ShowMainWindow_NotificationMenuItemSelected;


            var setTextMenuItem = new NotificationMenuItem("Help");
            setTextMenuItem.NotificationMenuItemSelected += Help_NotificationMenuItemSelected;

            var disableMenuItem = new NotificationMenuItem("Exit");
            disableMenuItem.NotificationMenuItemSelected += Exit_NotificationMenuItemSelected;


            //icon.AddMenuItem(hideMainWindow);
            icon.AddMenuItem(showMainWindow);
            icon.AddMenuItem(disableMenuItem);
            icon.AddMenuItem(setTextMenuItem);
        }

        public static void HideMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            if (App.RefMainWindow != null)
                Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Hide);
        }

        public static void ShowMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            if (App.RefMainWindow != null)
            {
                if (show)
                {
                    Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Hide);
                    menuItem.Text = "Show";
                }
                else
                {
                    Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Show);
                    menuItem.Text = "Hide";
                }
                show = !show;
            }
        }

        private static void Exit_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            if (App.RefMainWindow != null)
            {
                App.RefMainWindow.MainWindow.Closing -= Closing;
                Dispatcher.UIThread.InvokeAsync(() => { App.RefMainWindow.Shutdown(); });
                destroy = true;
            }
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
