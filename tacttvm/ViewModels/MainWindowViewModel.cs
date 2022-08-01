using System;
using tacttvm.Infrastructure.Commands;
using tacttvm.Models;
using tacttvm.Models.MyMemory;
using tacttvm.Service;
using tacttvm.Static;

namespace tacttvm.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            IAppPreferences? setting = SettingsService.Load();

            SetPreferences(setting);

            this.UrlService = new UrlService(setting, PrintInStatusBar, StaticResources.MyMemoryApiUrl);

            this.ReplacingSelectedTextService =
                new ReplacingSelectedTextService(
                    this.StyleOfWritingCompoundWords,
                    new TranslationMyMemory(UrlService.Url, PrintInStatusBar),
                    (x) => KeyNumberTemp = x, PrintInStatusBar, this.KeyNumber, this.ModifierMasks);


            UpdateSettingCommand = new LambdaCommand(OnUpdateSettingCommandExecuted, CanUpdateSettingCommandExecute);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void Closed(object? o, EventArgs e)
        {
            ReplacingSelectedTextService?.UnsubscribeToReceiveButtonNumber();
        }
        private void Open(object? o, EventArgs e)
        {
            ReplacingSelectedTextService?.SubscribeToReceiveButtonNumber();
        }
    }
}
