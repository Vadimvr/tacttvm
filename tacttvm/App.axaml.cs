using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Linq;
using tacttvm.ViewModels;
using tacttvm.Views;

namespace tacttvm
{
    public partial class App : Application, IDisposable
    {
        public static IClassicDesktopStyleApplicationLifetime? RefMainWindow { get; private set; }
       
        public static  MainWindowViewModel? mainWindowViewModel { get; private set; }

        public static App? app; 

        //   public static MainWindow RefMainWindow { get; private set; }

        public override void Initialize()
        {
            app = this;
            AvaloniaXamlLoader.Load(this);
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                mainWindowViewModel = new MainWindowViewModel();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                mainWindowViewModel.Desktop = desktop;
                desktop.MainWindow.Closing += (s, e) =>
                {
                    if (s != null)
                    {
                        ((Window)s).Hide();
                        e.Cancel = true;
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                };
                RefMainWindow = desktop;
               
            }
            base.OnFrameworkInitializationCompleted();
        }

       static public void DisposeStatic()
        {
            app?.Dispose();
        }

        public void Dispose()
        {
           GC.SuppressFinalize(this);
        }
    }

}
