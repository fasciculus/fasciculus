using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Fasciculus.Eve.PageModels
{
    public partial class SideBarModel : ObservableObject
    {
        [ObservableProperty]
        private bool ready = true;

        [RelayCommand]
        private Task Info()
        {
            return NavigateTo("//Info");
        }

        [RelayCommand]
        private Task Industry()
        {
            return NavigateTo("//Industry");
        }

        [RelayCommand]
        private Task Market()
        {
            return NavigateTo("//Market");
        }

        [RelayCommand]
        private Task Map()
        {
            return NavigateTo("//Map");
        }

        private async Task NavigateTo(string url)
        {
            Ready = false;

            await Task.Delay(100);
            await Shell.Current.GoToAsync(url);
            await Task.Delay(100);

            Ready = true;
        }
    }
}
