using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace tacttvm.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (AvaloniaLocator.Current.GetService<MainWindow>() == null)
            {
                AvaloniaLocator.CurrentMutable.Bind<MainWindow>().ToConstant(this);
            }
#if DEBUG
            this.AttachDevTools();
#endif

            // Prevent the main window from closing. Just hide it instead if we have a notify icon, or minimize it otherwise.

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Exits the app
        /// </summary>
        public void Exit()
        {
            (App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?
                .Shutdown(0);
        }
    }
}
