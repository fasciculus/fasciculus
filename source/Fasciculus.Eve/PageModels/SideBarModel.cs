using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class SideBarModel : ObservableObject
    {
#pragma warning disable CA1822 // Mark members as static
        [RelayCommand]
        private Task Navigate(string url)
        {
            return Shell.Current.GoToAsync(url)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(250)));
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
