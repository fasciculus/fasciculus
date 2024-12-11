using CommunityToolkit.Mvvm.Input;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;

namespace Fasciculus.Eve.ViewModels
{
    public partial class NavBarViewModel : MainThreadObservable
    {
        private readonly INavigator navigator;

        public NavBarViewModel(INavigator navigator)
        {
            this.navigator = navigator;
        }

        [RelayCommand]
        private Task GoTo(string url)
        {
            return navigator.GoTo(url);
        }
    }
}
