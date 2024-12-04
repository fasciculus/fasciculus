using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class MarketPageModel : MainThreadObservable
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        [ObservableProperty]
        private string hub;

        [ObservableProperty]
        private int maxDistance;

        [ObservableProperty]
        private int maxVolumePerType;

        [ObservableProperty]
        private int maxIskPerType;

        public MarketPageModel(SideBarModel sideBar, IEveResources resources)
        {
            this.sideBar = sideBar;

            hub = Tasks.Wait(resources.Universe).NpcStations[60003760].FullName;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
    }
}
