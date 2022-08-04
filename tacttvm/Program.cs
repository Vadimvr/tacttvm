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
            Log.Information("load from -> Assembly.LoadFrom tacttvm");
            Assembly[] assembles = AppDomain.CurrentDomain.GetAssemblies();
            Assembly? assembly = null;
            foreach (var names in assembles)
            {
                string? x = names.FullName;
                if (!string.IsNullOrWhiteSpace(x) && (x.Contains("tacttvm")))
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
    }
}