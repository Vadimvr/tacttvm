namespace tacttvm.Models
{
    /// <summary>
    /// This class exposes and manages application and user settings.
    /// </summary>
    public class AppSettings
    {
        private static AppSettings? _instance;
        private bool _startMinimized = true;

        /// <summary>
        /// Gets the singleton <c>AppSettings</c> instance.
        /// </summary>
        public static AppSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new AppSettings();
                return _instance;
            }
        }


        /// <summary>
        /// Gets or sets a value that determines if the app should start 
        /// minimized to the tray icon.
        /// </summary>
        public bool StartMinimized
        {
            get { return _startMinimized; }
            set
            {
                if (_startMinimized != value)
                {
                    _startMinimized = value;
                }
            }
        }

        /// <summary>
        /// The constructor is private, use the <Instance>property</Instance>
        /// to get the singletion class instance.
        /// </summary>
        private AppSettings()
        {
            Reload();
        }

        /// <summary>
        /// Discards any usaved changes and reloads all settings
        /// from the database.
        /// </summary>
        public void Reload()
        {
        }

    }
}
