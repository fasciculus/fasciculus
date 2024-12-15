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
    public interface IIndustry : INotifyPropertyChanged
    {
        public EveStation Hub { get; }

        public LongProgressInfo BuyProgressInfo { get; }
        public LongProgressInfo SellProgressInfo { get; }

        public Task StartAsync();
    }

    public partial class Industry : MainThreadObservable, IIndustry
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly EveIndustrySettings settings;
        private readonly IEsiClient esiClient;

        private readonly EveBlueprints blueprints;

        public EveStation Hub { get; }
        private readonly EveRegion hubRegion;

        [ObservableProperty]
        private LongProgressInfo buyProgressInfo = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgressInfo = LongProgressInfo.Start;

        private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        public Industry(IEveResources resources, IEveSettings settings, IEsiClient esiClient)
        {
            this.settings = settings.IndustrySettings;
            this.esiClient = esiClient;

            blueprints = Tasks.Wait(resources.Data).Blueprints;

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
            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            if (regionBuyOrders is null || regionSellOrders is null)
            {
                return;
            }

            EveBlueprint[] candidates = GetCandidates();
        }

        private EveBlueprint[] GetCandidates()
        {
            int maxVolume = settings.MaxVolume;
            EveSkills skills = new([]);

            return blueprints
                .Where(x => x.Manufacturing.Time <= 86400)
                .Where(x => x.Manufacturing.Products.All(y => y.Type.Volume <= maxVolume))
                .Where(x => skills.Fulfills(x.Manufacturing.Skills))
                .ToArray();
        }
    }
}
