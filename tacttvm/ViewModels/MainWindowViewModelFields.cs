using Avalonia.Controls.ApplicationLifetimes;
using SharpHook.Native;
using tacttvm.Models.MyMemory;
using tacttvm.Service;
using tacttvm.Service.StyleOfWritingCompoundWords;

namespace tacttvm.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private bool _CompoundWritingStyles;
        public bool CamelCase
        {
            get => _CompoundWritingStyles; set
            {
                this.Set(ref _CompoundWritingStyles, value);
                if (value)
                {
                    SnakeCase = false;
                }
            }
        }


        private bool _SnakeCase;
        public bool SnakeCase
        {
            get => _SnakeCase; set
            {
                this.Set(ref _SnakeCase, value);
                if (value)
                {
                    CamelCase = false;
                }
            }
        }


        private bool _Translation;
        public bool Translation { get => _Translation; set => Set(ref _Translation, value); }


        private string? _CurrentLanguag;
        public string? CurrentLanguage { get => _CurrentLanguag; set => Set(ref _CurrentLanguag, value); }


        private string? _LanguageTranslation;
        public string? LanguageTranslation { get => _LanguageTranslation; set => Set(ref _LanguageTranslation, value); }


        private string? _KeyChar;
        public string? KeyChar
        {
            get => _KeyChar; set
            {
                Set(ref _KeyChar, value);
                KeyNumber = KeyNumberTemp;
            }
        }
        public ModifierMask ModifierMasks { get; private set; }

        private bool _Alt;
        public bool Alt { get => _Alt; set => Set(ref _Alt, value); }


        private bool _Ctrl;
        public bool Ctrl { get => _Ctrl; set => Set(ref _Ctrl, value); }


        private bool _Shift;
        public bool Shift { get => _Shift; set => Set(ref _Shift, value); }


        private string? _Key;
        public string? ApiKey { get => _Key; set => Set(ref _Key, value); }
        public IStyleOfWritingCompoundWords? StyleOfWritingCompoundWords { get; private set; }

        private string? _StatsBar;
        public string? StatsBar { get => _StatsBar; set => Set(ref _StatsBar, value); }


        private ushort _KeyNumber;
        public ushort KeyNumber { get => _KeyNumber; set => Set(ref _KeyNumber, value); }

        public ushort KeyNumberTemp { get; set; }

        private string _Snake_Case = "Snake_Case";

        public string Snake_Case { get => _Snake_Case; set => Set(ref _Snake_Case, value); }

        private IClassicDesktopStyleApplicationLifetime? desktop;

        public IClassicDesktopStyleApplicationLifetime? Desktop
        {
            private get => desktop; set
            {
                desktop = value;
                if (desktop != null)
                {
                    desktop.MainWindow.Closing += this.Closed;
                    desktop.MainWindow.Opened += this.Open;
                }
            }
        }
        public UrlService UrlService { get; private set; }
        public ReplacingSelectedTextService ReplacingSelectedTextService { get; }
    }
}
