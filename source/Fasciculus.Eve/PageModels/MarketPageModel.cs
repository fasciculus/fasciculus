using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class MarketPageModel : MainThreadObservable
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        private readonly ITradeOptions tradeOptions;
        private readonly ITradeFinder tradeFinder;

        [ObservableProperty]
        private string targetHub;

        [ObservableProperty]
        private int maxDistance;

        [ObservableProperty]
        private int maxVolumePerType;

        [ObservableProperty]
        private int maxIskPerType;

        [ObservableProperty]
        private LongProgressInfo progress = LongProgressInfo.Start;

        [ObservableProperty]
        private EveTrade[] trades = [];

        public MarketPageModel(SideBarModel sideBar, ITradeOptions tradeOptions, ITradeFinder tradeFinder)
        {
            this.sideBar = sideBar;
            this.tradeOptions = tradeOptions;
            this.tradeFinder = tradeFinder;

            this.tradeFinder.PropertyChanged += OnTradeFinderChanged;

            targetHub = tradeOptions.TargetStation.FullName;
            maxDistance = tradeOptions.MaxDistance;
            maxVolumePerType = tradeOptions.MaxVolumePerType;
            maxIskPerType = tradeOptions.MaxIskPerType;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            tradeOptions.MaxDistance = MaxDistance;
            tradeOptions.MaxVolumePerType = MaxVolumePerType;
            tradeOptions.MaxIskPerType = MaxIskPerType;
        }

        private void OnTradeFinderChanged(object? sender, PropertyChangedEventArgs ev)
        {
            Progress = tradeFinder.Progress;
            Trades = tradeFinder.Trades.ShallowCopy();
        }

        [RelayCommand]
        private Task Start()
        {
            return tradeFinder.StartAsync();
        }
    }
}
