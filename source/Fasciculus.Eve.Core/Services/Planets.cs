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

        private readonly IEveResources resources;
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

        public Planets(IEveResources resources, IEsiClient esiClient)
        {
            this.resources = resources;
            this.esiClient = esiClient;

            schematics = Tasks.Wait(resources.Data).PlanetSchematics;

            Hub = Tasks.Wait(resources.Universe).Stations[60003760];
            hubRegion = Hub.GetRegion();

            buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            Productions = [];

            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            if (regionBuyOrders is null || regionSellOrders is null)
            {
                return;
            }

            EveStationBuyOrders buyOrders = regionBuyOrders[Hub];
            EveStationSellOrders sellOrders = regionSellOrders[Hub];
            EvePlanetProductions productions = new(schematics, buyOrders, sellOrders);

            Productions = productions.Take(10).ToArray();
        }
    }
}
