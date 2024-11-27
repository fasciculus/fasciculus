using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Fasciculus.Eve.PageModels
{
    public partial class SideBarModel : ObservableObject
    {
        [ObservableProperty]
        private bool ready = true;

        [RelayCommand]
        private async Task Info()
        {
            await NavigateTo("//Info");
        }

        [RelayCommand]
        private async Task Market()
        {
            await NavigateTo("//Market");
        }

        [RelayCommand]
        private async Task Map()
        {
            await NavigateTo("//Map");
        }

        private async Task NavigateTo(string url)
        {
            Ready = false;

            try
            {
                await Shell.Current.GoToAsync(url).ContinueWith(_ => { Ready = true; });
            }
            catch
            {
                Ready = true;
            }
        }
    }
}
