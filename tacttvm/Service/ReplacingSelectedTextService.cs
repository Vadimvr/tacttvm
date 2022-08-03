using SharpHook;
using SharpHook.Native;
using System;
using System.Threading;
using System.Threading.Tasks;
using tacttvm.Service.StyleOfWritingCompoundWords;

namespace tacttvm.Service
{
    public class ReplacingSelectedTextService
    {
        private SimpleGlobalHook hook;
        private EventSimulator simulator;
        private ModifierMask modifierMask;

        public IStyleOfWritingCompoundWords? StyleOfWriting { get; set; }

        public ReplacingSelectedTextService(
                IStyleOfWritingCompoundWords? styleOfWriting,
                ITranslationService translationService,
                Action<ushort> setKeyNumber,
                Action<string> printInStatusBar,
                ushort keyNumber, ModifierMask modifierMask)
        {
            this.KeyNumber = keyNumber;
            this.hook = new SimpleGlobalHook();
            this.simulator = new EventSimulator();
            this.StyleOfWriting = styleOfWriting;
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.hook.KeyTyped += OnKeyTyped;
            this.SetKeyNumber = setKeyNumber;
            this.PrintInStatusBar = printInStatusBar;
            this.ModifierMask = modifierMask;
            this.hook.RunAsync();
        }

        private bool Subscribe { get; set; }
        public ushort KeyNumber { get; set; }
        public ModifierMask ModifierMask
        {
            get => modifierMask; set
            {
                modifierMask = value == ModifierMask.None ? ModifierMask.LeftMeta | ModifierMask.LeftCtrl | ModifierMask.ScrollLock | ModifierMask.LeftAlt : value;
            }
        }
        public bool WordProcessing { get; private set; }
        public ITranslationService TranslationService { get; set; }

        internal void Exit()
        {
            this.hook.Dispose();
        }

        public Action<ushort> SetKeyNumber { get; }
        public Action<string> PrintInStatusBar { get; }

        private void OnKeyTyped(object? sender, KeyboardHookEventArgs e)
        {
            if (e.RawEvent.Keyboard.RawCode == this.KeyNumber && e.RawEvent.Mask.HasFlag(this.ModifierMask))
            {
                e.Reserved = EventReservedValueMask.SuppressEvent;
                if (WordProcessing)
                {
                    return;
                }
                else
                {
                    WordProcessing = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            // Old buffer;
                            string oldBuffer = TextCopy.ClipboardService.GetText() ?? string.Empty;

                            //copy selected text
                            Hotkeys(KeyCode.VcC, e.RawEvent.Mask);

                            // new buffer
                            string SelectedText = TextCopy.ClipboardService.GetText() ?? String.Empty;
                            // Selected text is missing => break 
                            if (string.IsNullOrWhiteSpace(SelectedText))
                            {
                                TextCopy.ClipboardService.SetText(oldBuffer);
                                return;
                            }
                            // translated text from MyMemoryApi
                            string translatedText = StyleOfWriting?.GetTetx(TranslationService.GetText(SelectedText)) ?? string.Empty;

                            // Paste the text into the buffer
                            TextCopy.ClipboardService.SetText(translatedText);

                            // paste text
                            Hotkeys(KeyCode.VcV, e.RawEvent.Mask);

                            // return the old buffer
                            TextCopy.ClipboardService.SetText(oldBuffer);
                        }
                        catch (Exception ex)
                        {
                            this.hook?.Dispose();

                            var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                                .GetMessageBoxStandardWindow("Exception", ex.Message);
                            messageBoxStandardWindow.Show();
                        }
                        finally
                        {
                            WordProcessing = false;
                        }
                    });
                }
            }
        }

        public void SubscribeToReceiveButtonNumber()
        {
            if (this.hook != null)
            {
                if (this.Subscribe)
                {
                    this.hook.KeyTyped += GetKeyNumber;
                    this.Subscribe = false;
                }
            }
        }

        public void UnsubscribeToReceiveButtonNumber()
        {
            if (this.hook != null)
            {
                if (!this.Subscribe)
                {
                    this.hook.KeyTyped -= GetKeyNumber;
                    this.Subscribe = true;
                }
            }
        }
        private void GetKeyNumber(object? sender, KeyboardHookEventArgs e)
        {
            this.SetKeyNumber?.Invoke(e.RawEvent.Keyboard.RawCode);
        }

        private void Hotkeys(KeyCode key, ModifierMask modifier)
        {
            if (modifier.HasFlag(ModifierMask.LeftShift))
                simulator.SimulateKeyRelease(KeyCode.VcLeftShift);
            if (modifier.HasFlag(ModifierMask.LeftAlt))
                simulator.SimulateKeyRelease(KeyCode.VcLeftAlt);

            simulator.SimulateKeyPress(KeyCode.VcLeftControl);
            simulator.SimulateKeyPress(key);
            Thread.Sleep(50);
            simulator.SimulateKeyRelease(key);
            simulator.SimulateKeyRelease(KeyCode.VcLeftControl);
        }
    }
}
