using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetaryIndustryPageModel : MainThreadObservable
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        public PlanetaryIndustryPageModel(SideBarModel sideBar)
        {
            this.sideBar = sideBar;
        }
    }
}
