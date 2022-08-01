using ReactiveUI;
using System.Runtime.CompilerServices;

namespace tacttvm.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected virtual T Set<T>(ref T field, T value, [CallerMemberName] string? PropertyName = null)
        {
           return  this.RaiseAndSetIfChanged( ref field,value,PropertyName);   
        }
    }
}
