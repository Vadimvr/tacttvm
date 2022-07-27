using tacttvm.Infrastructure.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;

namespace tacttvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
          
        ViewModelBase _content;
        ViewModelBase _priorContent;

        private string _KeyCombination = "ctr + E";

        public string KeyCombination { get => _KeyCombination; set => this.RaiseAndSetIfChanged(ref _KeyCombination, value); }

        private string _fieldName = "Hello World";

        public string FieldName { get => _fieldName; set => this.RaiseAndSetIfChanged(ref _fieldName, value); }

        public MainWindowViewModel()
        {

        }

        /// <summary>
        /// Gets or sets the view model of the corresponding view that will 
        /// be displayed within the app's main window.
        /// </summary>
       


    }
}
