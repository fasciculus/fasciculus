using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Fasciculus.Eve.PageModels
{
    public partial class SideBarModel : ObservableObject
    {
        [RelayCommand]
        private async Task Info()
        {
            await Shell.Current.GoToAsync("//Info");
        }

        [RelayCommand]
        private async Task Market()
        {
            await Shell.Current.GoToAsync("//Market");
        }

        [RelayCommand]
        private async Task Map()
        {
            await Shell.Current.GoToAsync("//Map");
        }
    }
}
