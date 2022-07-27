using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using NotificationIconSharp;
using System;
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


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        const string icon_path = @"D:\test\1.ico";

        static bool destroy = false;
        private static Task task;
        public static Action hide;

        public static string[] Args { get; set; }
       
        static void AppMain(Application app, string[] args)
        {
            // A cancellation token source that will be used to stop the main loop
            var cts = new CancellationTokenSource();

            // Do you startup code here
            new Window().Show();

            // Start the main loop
            app.Run(cts.Token);
        }

        static void Main(string[] args)
        {
            Args = args;

            NotificationManager.Initialize("com.app.test", icon_path, icon_path);
            // NotificationManager.SendNotification("My New Notification", "Isn't This Handy", "ActionId", icon_path);
            NotificationManager.NotificationIconSelectedEvent += NotificationManager_NotificationIconSelectedEvent;

            var trayIcon = new NotificationIcon(icon_path);
            TrayIcon_NotificationIconSelected(trayIcon);
            //trayIcon.NotificationIconSelected += TrayIcon_NotificationIconSelected;
            task = new Task(() =>
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            });
            task.Start();
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

        private static void NotificationManager_NotificationIconSelectedEvent(string notificationId)
        {
            Console.WriteLine(notificationId);
        }

        private static void TrayIcon_NotificationIconSelected(NotificationIcon icon)
        {
            if (icon.MenuItems.Count > 0) return;


            var hideMainWindow = new NotificationMenuItem("hide");
            hideMainWindow.NotificationMenuItemSelected += HideMainWindow_NotificationMenuItemSelected;

            var showMainWindow = new NotificationMenuItem("Open");
            showMainWindow.NotificationMenuItemSelected += ShowMainWindow_NotificationMenuItemSelected;

            var disableMenuItem = new NotificationMenuItem("exit");
            disableMenuItem.NotificationMenuItemSelected += Exit_NotificationMenuItemSelected;

            var setTextMenuItem = new NotificationMenuItem("Help");
            setTextMenuItem.NotificationMenuItemSelected += Help_NotificationMenuItemSelected;

          
            icon.AddMenuItem(hideMainWindow);
            icon.AddMenuItem(showMainWindow);
            icon.AddMenuItem(disableMenuItem);
            icon.AddMenuItem(setTextMenuItem);
        }

        private static void CheckMenuItem_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Hide);
 
        }
        public static void HideMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Hide);

        }
        public static void ShowMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Show);

        }

        private static void Exit_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            destroy = true;
        }

        private static void Help_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            menuItem.Text = "Hello World!";
        }

        private static void SubMenuMenuItem_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        {
            if (menuItem.MenuItems.Count > 0) return;

            var destroyMenuItem = new NotificationMenuItem("Destroy Menu");
            destroyMenuItem.NotificationMenuItemSelected += DestroyMenuItem_NotificationMenuItemSelected;
            menuItem.AddMenuItem(destroyMenuItem);
        }

        private static void DestroyMenuItem_NotificationMenuItemSelected(NotificationMenuItem icon)
        {
            
            destroy = true;
        }
    }


}
