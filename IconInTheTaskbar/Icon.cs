using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using IconInTheTaskbar.Models;
using IconInTheTaskbar.Models.IPC;
using IconInTheTaskbar.Platform;
using MonoMac.AppKit;
using System.ComponentModel;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IconInTheTaskbar
{
    public class IconCreate
    {
        public static Action<string>? LogInformation;
        public static INotifyIcon? NotifyIcon { get; set; } = null;
        public static INotifyIconParameters? NotifyIconParameters { get; set; } = null;
        public static Assembly? Assembly { get; set; }

        public static void OnFrameworkInitializationCompletedinApp(
            IClassicDesktopStyleApplicationLifetime desktop,
            Window? mainWindow,
           ref INotifyIcon? notifyIconFromApp,
            INotifyIconParameters NotifyIconParameters,
            List<MenuItem> menuItems,
            ContextMenu? notifyIconContextMenu,
            bool startMinimized,
            Action RestoreMainWindow)
        {

            

            if (NotifyIconParameters == null)
            {
                LogInformation?.Invoke("NotifyIconParameters is null");
                throw new ArgumentNullException(nameof(NotifyIconParameters));
            }


            // If this is running on a Mac we need a special event handler for URL schema invokation
            // This also handles System Events and notifications, it gives us a native foothold on a Mac.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                LogInformation?.Invoke("Initialializing App Delegate for macOS");
                NSApplication.Init();
                NSApplication.SharedApplication.Delegate = new IconInTheTaskbar.Platform.OSX.AppDelegate();
            }

            // Set up and configure the notification icon
            // Get the type of the platform-specific implementation
            Type type = Implementation.ForType<INotifyIcon>();
            if (type != null)
            {
                // If we have one, create an instance for it
                NotifyIcon = Activator.CreateInstance(type) as INotifyIcon;
                LogInformation?.Invoke(NotifyIcon.ToString());
            }

            if (NotifyIcon != null)
            {
                LogInformation?.Invoke("NotifyIcon implementation available, setting up NotifyIcon");

                NotifyIcon.ToolTipText = NotifyIconParameters.ApplicationNameInTheTaskbar;
                NotifyIcon.IconPath = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
                                      NotifyIconParameters.ApplicationIconInTheTaskbarOSX :
                                      RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                      NotifyIconParameters.ApplicationIconInTheTaskbarWin:
                                      NotifyIconParameters.ApplicationIconInTheTaskbarLin;

                NotifyIcon.DoubleClick += (s, e) =>
                {
                    RestoreMainWindow?.Invoke();
                };

                notifyIconContextMenu = new ContextMenu();

                notifyIconContextMenu.Items = menuItems;
                NotifyIcon.ContextMenu = notifyIconContextMenu;
                NotifyIcon.Visible = true;
            }
            // We establish and keep a Quick Pass Manager instance here so that it will immediately receive system event notifications
            //  _quickPassManager = QuickPassManager.Instance;

            // Set up the app's main window, if we aren't staring minimized to tray
            if (!startMinimized || NotifyIcon == null)
            {
                desktop.MainWindow = mainWindow;
            }
            notifyIconFromApp = NotifyIcon;
        }

        public static void MainInProgram(
            string mutexId,
            string[] args,
            bool checkingRunningInstance,
            Action HandleAbruptCPS,
            Action<object?, EventArgs> currentDomain_ProcessExit,
            Action<string[]> buildAvaloniaApp,
            Action RestoreMainWindow)
        {
            if (NotifyIconParameters == null)
            {
                LogInformation?.Invoke("NotifyIconParameters is null");
                throw new ArgumentNullException(nameof(NotifyIconParameters));
            }


            Thread ipcThread = new Thread((x) => StartIPCServer(x, NotifyIconParameters, RestoreMainWindow));

            // Checks if a duplicate of this application is running
            using (var mutex = new Mutex(false, mutexId, out bool created))
            {
                bool hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(500, false);
                        if (!hasHandle && checkingRunningInstance)
                        {
                            // Existing instance detected, forward the first 
                            // command line argument if present.
                            LogInformation?.Invoke("Existing app instance detected, forwarding data and shutting down");
                            ForwardToExistingInstance(args.Length > 0 ? args[0] : IPCServer.MAGIC_WAKEUP_STR, NotifyIconParameters);
                            Environment.Exit(1);
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        hasHandle = true;
                    }

                    // Adds event to handle abrupt program exits and mitigate CPS
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(currentDomain_ProcessExit);

                    // No existing instance of the app running,
                    // so start the IPC server and run the app
                    if (checkingRunningInstance)
                    {
                        ipcThread.Start();
                    }
                    
                    buildAvaloniaApp(args);
                }
                finally
                {
                    LogInformation?.Invoke("App shutting down");

                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }

                    HandleAbruptCPS();

                    //Remove the notify icon
                    NotifyIcon?.Remove();

                    if (ipcThread.IsAlive)
                    {
                        // Force close the app without waiting for any threads to finish.
                        LogInformation?.Invoke("Forcing exit because of IPC thread still running.");
                        Environment.Exit(1);
                    }
                }
            }
        }

        /// <summary>
        /// Starts the IPC server and also starts listening for incoming IPC queries.
        /// </summary>
        /// <param name="obj">Not used.</param>
        private static void StartIPCServer(object? obj, INotifyIconParameters notifyIconParameters, Action RestoreMainWindow)
        {
            IPCServer nps = new IPCServer(notifyIconParameters.LocalIp, notifyIconParameters.LocalPort, RestoreMainWindow);
            nps.StartListening();
        }

        /// <summary>
        /// Forwards the given string to the existing app instance by estblishing
        /// a TCP connection to the existing instance's IPC server and sending the
        /// given data over that connection.
        /// </summary>
        /// <param name="url">The data to send to the exisnting app instance.</param>
        private static void ForwardToExistingInstance(string url, INotifyIconParameters notifyIconParameters)
        {
            try
            {
                TcpClient client = new TcpClient(notifyIconParameters.LocalIp, notifyIconParameters.LocalPort);
                NetworkStream stream = client.GetStream();

                // Translate the message into ASCII.
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(url);

                // Send the message to the connected TCP server. 
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }


        public static void WindowClosing(object? s, CancelEventArgs e)
        {
            //if (NotifyIcon != null)
            //{
            //    Log.Information("Hiding main window instead of closing it");
            //    Dispatcher.UIThread.Post(() =>
            //    {
            //        ((Window)s).Hide();
            //    });
            //    (App.Current as App).NotifyIcon.Visible = true;
            //    App.Minimized = !App.Minimized;
            //}
            //else
            //{
            //    Log.Information("Minimizing main window instead of closing it");
            //    Dispatcher.UIThread.Post(() =>
            //    {
            //        ((Window)s).WindowState = WindowState.Minimized;
            //    });
            //}
            //e.Cancel = true;
        }
    }
}
