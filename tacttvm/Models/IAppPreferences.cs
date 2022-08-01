using SharpHook.Native;
using tacttvm.Service.StyleOfWritingCompoundWords;

namespace tacttvm.Models
{
    public interface IAppPreferences
    {
        string? ApiKey { get; set; }
        string? CurrentLanguage { get; set; }
        string? TranslationLanguage { get; set; }
        ModifierMask ModifierMasks { get; set; }
        ushort KeyNumber { get; set; }
        StyleOfWritingCompoundWordsEnum StyleOfWritingCompoundWords { get; set; }
    }
}