using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        [ObservableProperty]
        private bool startEnabled = true;

        [RelayCommand]
        private async Task Start()
        {
            StartEnabled = false;

            await Task.Delay(1000);

            StartEnabled = true;
        }
    }
}
