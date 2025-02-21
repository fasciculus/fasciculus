using Fasciculus.Eve.Models;
using Fasciculus.Progress;
using Fasciculus.Threading.Synchronization;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEsiClient
    {
        public Task<EveMarketPrices?> GetMarketPricesAsync();

        public Task<EveRegionBuyOrders?> GetRegionBuyOrdersAsync(EveRegion region, IAccumulatingProgress<long> progress);
        public Task<EveRegionSellOrders?> GetRegionSellOrdersAsync(EveRegion region, IAccumulatingProgress<long> progress);

        public Task<EveIndustryIndices?> GetIndustryIndicesAsync();
    }

    public partial class EsiClient : IEsiClient
    {
        private readonly IEsiCache esiCache;
        private readonly IEsiHttp esiHttp;

        private readonly EveTypes types;
        private readonly EveSolarSystems solarSystems;
        private readonly EveStations stations;

        private readonly TaskSafeMutex mutex = new();

        public EsiClient(IEsiCache esiCache, IEsiHttp esiHttp, IEveProvider provider)
        {
            this.esiCache = esiCache;
            this.esiHttp = esiHttp;

            types = provider.Types;
            solarSystems = provider.SolarSystems;
            stations = provider.Stations;
        }
    }
}
