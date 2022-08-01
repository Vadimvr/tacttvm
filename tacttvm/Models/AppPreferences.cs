using SharpHook.Native;
using tacttvm.Service.StyleOfWritingCompoundWords;

namespace tacttvm.Models
{
    public class AppPreferences : IAppPreferences
    {
        public bool Translation { get; set; }
        public string? CurrentLanguage { get; set; }
        public string? TranslationLanguage { get; set; }
        public string? ApiKey { get; set; }
        public ModifierMask ModifierMasks { get; set; }
        public ushort KeyNumber { get; set; }
        public StyleOfWritingCompoundWordsEnum StyleOfWritingCompoundWords { get; set; }
    }
}