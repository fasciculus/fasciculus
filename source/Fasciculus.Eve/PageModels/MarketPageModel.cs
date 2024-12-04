using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class MarketPageModel : MainThreadObservable
    {
        private readonly ITradeOptions tradeOptions;

        [ObservableProperty]
        private SideBarModel sideBar;

        [ObservableProperty]
        private string targetHub;

        [ObservableProperty]
        private int maxDistance;

        [ObservableProperty]
        private int maxVolumePerType;

        [ObservableProperty]
        private int maxIskPerType;

        public MarketPageModel(SideBarModel sideBar, ITradeOptions tradeOptions)
        {
            this.tradeOptions = tradeOptions;
            this.sideBar = sideBar;

            targetHub = tradeOptions.TargetStation.FullName;
            maxDistance = tradeOptions.MaxDistance;
            maxVolumePerType = tradeOptions.MaxVolumePerType;
            maxIskPerType = tradeOptions.MaxIskPerType;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            tradeOptions.MaxDistance = MaxDistance;
            tradeOptions.MaxVolumePerType = MaxVolumePerType;
            tradeOptions.MaxIskPerType = MaxIskPerType;
        }
    }
}
