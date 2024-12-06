﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
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
        private string targetHub = string.Empty;

        [ObservableProperty]
        private int maxDistance;

        [ObservableProperty]
        private int maxVolumePerType;

        [ObservableProperty]
        private int maxIskPerType;

        [ObservableProperty]
        private string progress = string.Empty;

        [ObservableProperty]
        private EveTrade[] trades = [];

        public MarketPageModel(SideBarModel sideBar, ITradeOptions tradeOptions, ITradeFinder tradeFinder)
        {
            this.sideBar = sideBar;
            this.tradeOptions = tradeOptions;
            this.tradeFinder = tradeFinder;

            this.tradeOptions.PropertyChanged += OnTradeOptionsChanged;
            this.tradeFinder.PropertyChanged += OnTradeFinderChanged;

            EveTradeOptions options = tradeOptions.Options;

            targetHub = options.TargetStation.FullName;
            maxDistance = options.MaxDistance;
            maxVolumePerType = options.MaxVolumePerType;
            maxIskPerType = options.MaxIskPerType;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            tradeOptions.Options = tradeOptions.Create(MaxDistance, MaxVolumePerType, MaxIskPerType);
        }

        private void OnTradeOptionsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            EveTradeOptions options = tradeOptions.Options;

            TargetHub = options.TargetStation.FullName;
            MaxDistance = options.MaxDistance;
            MaxVolumePerType = options.MaxVolumePerType;
            MaxIskPerType = options.MaxIskPerType;
        }

        private void OnTradeFinderChanged(object? sender, PropertyChangedEventArgs ev)
        {
            switch (ev.PropertyName)
            {
                case nameof(ITradeFinder.Progress):
                    Progress = tradeFinder.Progress;
                    break;

                case nameof(ITradeFinder.Trades):
                    Trades = tradeFinder.Trades.ShallowCopy();
                    break;
            }

        }

        [RelayCommand]
        private Task Start()
        {
            return tradeFinder.StartAsync();
        }
    }
}
