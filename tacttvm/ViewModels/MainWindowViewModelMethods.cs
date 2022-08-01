using SharpHook.Native;
using System;
using tacttvm.Models;
using tacttvm.Models.MyMemory;
using tacttvm.Service;
using tacttvm.Service.StyleOfWritingCompoundWords;
using tacttvm.Static;

namespace tacttvm.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private void SetPreferences(IAppPreferences? appSettings)
        {
            if (appSettings != null)
            {
                this.CurrentLanguage = appSettings.CurrentLanguage;
                this.LanguageTranslation = appSettings.TranslationLanguage;
                this.KeyNumber = appSettings.KeyNumber;
                this.ModifierMasks = appSettings.ModifierMasks;
                this.ApiKey = appSettings.ApiKey;
                this.StyleOfWritingCompoundWords = StyleOfWritingCompoundWordsSetter.Get(appSettings.StyleOfWritingCompoundWords);

            }

            if (this.StyleOfWritingCompoundWords == null
                || this.StyleOfWritingCompoundWords is tacttvm.Service.StyleOfWritingCompoundWords.CamelCase)
            {
                this.StyleOfWritingCompoundWords = StyleOfWritingCompoundWordsSetter.Get(StyleOfWritingCompoundWordsEnum.CamelCase);
                CamelCase = true;
            }
            else
            {
                this.StyleOfWritingCompoundWords = StyleOfWritingCompoundWordsSetter.Get(StyleOfWritingCompoundWordsEnum.SnakeCase);
                SnakeCase = true;
            }
            this.Alt = ModifierMasks.HasFlag(ModifierMask.LeftAlt);
            this.Shift = ModifierMasks.HasFlag(ModifierMask.LeftShift);
            this.Ctrl = ModifierMasks.HasFlag(ModifierMask.LeftCtrl);
        }



        private IAppPreferences SevePreferences()
        {

            return new AppPreferences()
            {
                CurrentLanguage = this.CurrentLanguage,
                TranslationLanguage = this.LanguageTranslation,
                ModifierMasks = this.ModifierMasks,
                ApiKey = this.ApiKey,
                Translation = this.Translation,
                KeyNumber = this.KeyNumber
            };
        }

        internal void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            SettingsService.Save(SevePreferences());
            if (ReplacingSelectedTextService != null)
            {
                this.ReplacingSelectedTextService.Exit();
            }
        }

        public void PrintInStatusBar(string message)
        {
            this.StatsBar = $"{DateTime.Now}  {message}";
        }
    }
}
