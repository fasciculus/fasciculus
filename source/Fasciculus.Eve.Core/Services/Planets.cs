using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using Fasciculus.Threading;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IPlanets : INotifyPropertyChanged
    {
        public EveStation Hub { get; }

        public LongProgressInfo BuyProgressInfo { get; }
        public LongProgressInfo SellProgressInfo { get; }

        public EvePlanetProduction[] Productions { get; }

        public Task StartAsync();
    }

    public partial class Planets : MainThreadObservable, IPlanets
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly EvePlanetsSettings settings;
        private readonly IEsiClient esiClient;

        private readonly EvePlanetSchematics schematics;

        public EveStation Hub { get; }
        private readonly EveRegion hubRegion;

        [ObservableProperty]
        private LongProgressInfo buyProgressInfo = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgressInfo = LongProgressInfo.Start;

        private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        [ObservableProperty]
        private EvePlanetProduction[] productions = [];

        public Planets(IEveSettings settings, IEsiClient esiClient, IEveProvider provider)
        {
            this.settings = settings.PlanetsSettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.esiClient = esiClient;

            schematics = provider.PlanetSchematics;

            Hub = provider.Stations[60003760];
            hubRegion = Hub.GetRegion();

            buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Reset();
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Reset()
        {
            Productions = [];
        }

        private void Start()
        {
            Reset();

            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            if (regionBuyOrders is null || regionSellOrders is null)
            {
                return;
            }

            double customsTaxRate = settings.CustomsTaxRate / 1000.0;
            double salesTaxRate = settings.SalesTaxRate / 1000.0;
            EveStationBuyOrders buyOrders = regionBuyOrders[Hub];
            EveStationSellOrders sellOrders = regionSellOrders[Hub];
            EvePlanetProductions productions = new(schematics, buyOrders, sellOrders, customsTaxRate, salesTaxRate);

            Productions = productions.Take(10).ToArray();
        }
    }
}
