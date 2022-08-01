using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using testTryi.ViewModels;
using testTryi.Views;

namespace testTryi
{
    public partial class App : Application
    {
        public static IClassicDesktopStyleApplicationLifetime? RefMainWindow { get; private set; }

        //   public static MainWindow RefMainWindow { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }



        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                    
                };
                
                
                RefMainWindow = desktop;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

     
}
