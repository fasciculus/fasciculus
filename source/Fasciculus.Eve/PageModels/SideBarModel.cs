using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Maui.Services;

namespace Fasciculus.Eve.PageModels
{
    public partial class SideBarModel : ObservableObject
    {
        private readonly INavigator navigator;

        public SideBarModel(INavigator navigator)
        {
            this.navigator = navigator;
        }

        [RelayCommand]
        private Task Navigate(string url)
        {
            return navigator.GoTo(url);
        }
    }
}
