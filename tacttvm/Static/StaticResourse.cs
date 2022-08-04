
using IconInTheTaskbar;

namespace tacttvm.Static
{
    public static class StaticResources
    {
        public const string MyMemoryApiUrl = "https://api.mymemory.translated.net";
        private static NotifyIconParameters? notifyIconParameters;
        public static string MutexId { get; set; } = @"Global\{{f0d6b2c6-9f88-4324-89ea-5d8b4cba372e}}";
        public static string ApplicationNameInTheTaskbar { get; set; } = "App name";
        public static string ApplicationIconInTheTaskbarOSX { get; set; } = @"resm:tacttvm.Assets.tacttvm_osx_icon.png";
        public static string ApplicationIconInTheTaskbarWin { get; set; } = @"resm:tacttvm.Assets.tacttvm_win_icon.ico";
        public static string ApplicationIconInTheTaskbarLin { get; set; } = @"resm:tacttvm.Assets.tacttvm_lin_icon.ico";

        // address and port for application listening on duplicate launch 
        // if the application is running, then the second instance will not start 
        public static string LocalIp { get; internal set; } = "127.0.0.1";
        public static int LocalPort { get; internal set; } = 13000;
        public static NotifyIconParameters NotifyIconParameters
        {
            get
            {
                if (notifyIconParameters == null) notifyIconParameters = new NotifyIconParameters();
                return notifyIconParameters;
            }
        }
    }


    public class NotifyIconParameters : INotifyIconParameters
    {
        public string ApplicationNameInTheTaskbar { get => StaticResources.ApplicationNameInTheTaskbar; }
        public string ApplicationIconInTheTaskbarOSX { get => StaticResources.ApplicationIconInTheTaskbarOSX; }
        public string ApplicationIconInTheTaskbarWin { get => StaticResources.ApplicationIconInTheTaskbarWin; }
        public string ApplicationIconInTheTaskbarLin { get => StaticResources.ApplicationIconInTheTaskbarLin; }

        public string LocalIp { get => StaticResources.LocalIp; }

        public int LocalPort { get => StaticResources.LocalPort; }
    }
}
