using Avalonia;
using Avalonia.Dialogs;
using Avalonia.ReactiveUI;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using tacttvm.Static;

namespace tacttvm
{
    internal class Program
    {
        //v1.0.0.1
        static void Main(string[] args)
        {

            bool checkingRunningInstance = true;

            // Set up logging
            SetUpLogging();
            Log.Information("");
            Log.Information("load from -> Assembly.LoadFrom AvaloniaApplication1");
            Assembly[] assembles = AppDomain.CurrentDomain.GetAssemblies();
            Assembly? assembly = null;
            foreach (var names in assembles)
            {
                string? x = names.FullName;
                if (!string.IsNullOrWhiteSpace(x) && (x.Contains("AvaloniaApplication1")))
                {
                    assembly = names;
                }
            }


            IconInTheTaskbar.IconCreate.Assembly = assembly;
            IconInTheTaskbar.IconCreate.LogInformation += Log.Information;
            IconInTheTaskbar.IconCreate.NotifyIconParameters = StaticResources.NotifyIconParameters;
            IconInTheTaskbar.IconCreate.MainInProgram(
                StaticResources.MutexId,
                args,
                checkingRunningInstance,
                HandleAbruptCPS,
                CurrentDomain_ProcessExit,
                BuildAvaloniaApp,
                () => (App.Current as App)?.RestoreMainWindow()
                );
        }
        private static void SetUpLogging()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string logFilePath = Path.Combine(currentDir, "log.txt");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("New app instance is being launched on {OSDescription}", RuntimeInformation.OSDescription);
        }

        private static void BuildAvaloniaApp(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, Avalonia.Controls.ShutdownMode.OnExplicitShutdown);
        }


        /// <summary>
        /// Try to capture abrupt process exit to gracefully handle CPS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            // One of the last ditch efforts at gracefully handling CPS, note there may be no localization here we are at this point throwing a hail marry
            HandleAbruptCPS();
        }

        /// <summary>
        /// Handles unclean process exit tries to save CPS
        /// </summary>
        private static void HandleAbruptCPS()
        {
            try
            {
                Log.Information("Attempting to End CPS Gracefully from Process Exit Event");
            }
            catch (Exception ex)
            {
                Log.Error("Failed to Cancel CPS Gracefully", ex);
            }
        }


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new AvaloniaNativePlatformOptions { UseGpu = !RuntimeInformation.IsOSPlatform(OSPlatform.OSX) })
                .LogToTrace()
                .UseReactiveUI()
                .UseManagedSystemDialogs(); //It is recommended by Avalonia Developers that we use Managed System Dialogs instead  of the native ones particularly for Linux

        //    task = new Task(() =>
        //    {
        //        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        //    });
        //    task.Start();

        //    NotificationManager.Initialize("com.app.test", icon_path, icon_path);
        //    NotificationManager.NotificationIconSelectedEvent += NotificationManager_NotificationIconSelectedEvent;

        //    var trayIcon = new NotificationIcon(icon_path);
        //    TrayIcon_NotificationIconSelected(trayIcon);
        //    do
        //    {
        //        if (App.RefMainWindow != null)
        //        {
        //            Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Close);
        //            break;
        //        }
        //    } while (true);

        //    while (!destroy)
        //    {

        //        trayIcon?.DoMessageLoop(true);
        //    }

        //    trayIcon?.Dispose();

        //    Dispatcher.UIThread.InvokeAsync(() => App.RefMainWindow.Shutdown(1));

        //    App.DisposeStatic();
        //}

        //private static void NotificationManager_NotificationIconSelectedEvent(string notificationId)
        //{
        //    Console.WriteLine(notificationId);
        //}

        //private static void TrayIcon_NotificationIconSelected(NotificationIcon icon)
        //{
        //    if (icon.MenuItems.Count > 0) return;

        //    var showMainWindow = new NotificationMenuItem("Show");
        //    showMainWindow.NotificationMenuItemSelected += ShowMainWindow_NotificationMenuItemSelected;


        //    var setTextMenuItem = new NotificationMenuItem("Help");
        //    setTextMenuItem.NotificationMenuItemSelected += Help_NotificationMenuItemSelected;

        //    var disableMenuItem = new NotificationMenuItem("Exit");
        //    disableMenuItem.NotificationMenuItemSelected += Exit_NotificationMenuItemSelected;

        //    icon.AddMenuItem(showMainWindow);
        //    icon.AddMenuItem(disableMenuItem);
        //    icon.AddMenuItem(setTextMenuItem);
        //}

        //public static void ShowMainWindow_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        //{
        //    if (App.RefMainWindow == null)
        //    {
        //        throw new ArgumentNullException(nameof(App.RefMainWindow), "App.RefMainWindow is null");
        //    }
        //    if (show)
        //    {
        //        Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Close);
        //        menuItem.Text = "Show";
        //    }
        //    else
        //    {
        //        Dispatcher.UIThread.InvokeAsync(App.RefMainWindow.MainWindow.Show);
        //        menuItem.Text = "Hide";
        //    }

        //    show = !show;
        //}

        //private static void Exit_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        //{
        //    destroy = true;
        //}

        //private static void Help_NotificationMenuItemSelected(NotificationMenuItem menuItem)
        //{
        //    Dispatcher.UIThread.InvokeAsync(() =>
        //    {
        //        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
        //        .GetMessageBoxStandardWindow("Code", "https://github.com/Vadimvr/tacttvm");
        //        messageBoxStandardWindow.Show();
        //    });
        //}
    }
}