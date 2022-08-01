using SharpHook.Native;
using System.Windows.Input;
using tacttvm.Models;
using tacttvm.Models.MyMemory;
using tacttvm.Service;
using tacttvm.Service.StyleOfWritingCompoundWords;
using translationWord;

namespace tacttvm.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        ITranslationParameters? translationParameters { get; set; }

        private void LaunchTranslator()
        {
        }

        public ICommand UpdateSettingCommand { get; }

        private bool CanUpdateSettingCommandExecute(object p)
        {
            return ReplacingSelectedTextService != null;
        }
        private void OnUpdateSettingCommandExecuted(object p)
        {
            var x = ModifierMask.None;
            if (Alt)
                x |= ModifierMask.LeftAlt;
            if (Shift)
                x |= ModifierMask.LeftShift;
            if (Ctrl)
                x |= ModifierMask.LeftCtrl;

            if (x == ModifierMask.None)
            {
                PrintInStatusBar("Select alt ctrl or shift");
                return;
            }
            else
            {
               this. ModifierMasks = x;
            }

            if (string.IsNullOrWhiteSpace(this.LanguageTranslation))
            {
                PrintInStatusBar("Enter the target language. For example \"en\"");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.CurrentLanguage))
            {
                PrintInStatusBar("Enter the current language. For example \"en\"");
                return;
            }

            this.UrlService.CreateUrl(this.CurrentLanguage, this.LanguageTranslation, Static.StaticResources.MyMemoryApiUrl, this.ApiKey);
            this.ReplacingSelectedTextService.TranslationService.UrlService = this.UrlService.Url;
            this.ReplacingSelectedTextService.KeyNumber = this.KeyNumber;
            this.ReplacingSelectedTextService.ModifierMask = x;
            this.ReplacingSelectedTextService.StyleOfWriting = this.CamelCase ? new CamelCase() : new SnakeCase();

            SettingsService.Save(SevePreferences());
        }
    }
}
