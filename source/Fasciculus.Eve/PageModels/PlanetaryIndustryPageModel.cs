using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetaryIndustryPageModel : MainThreadObservable
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        [ObservableProperty]
        private StatusBarModel statusBar;

        private readonly IPlanetaryIndustry planetaryIndustry;

        public PlanetaryIndustryPageModel(SideBarModel sideBar, IPlanetaryIndustry planetaryIndustry)
        {
            this.sideBar = sideBar;

            statusBar = Application.Current!.Handler.GetRequiredService<StatusBarModel>();

            this.planetaryIndustry = planetaryIndustry;
        }

        [RelayCommand]
        private Task Start()
        {
            return planetaryIndustry.StartAsync();
        }
    }
}
