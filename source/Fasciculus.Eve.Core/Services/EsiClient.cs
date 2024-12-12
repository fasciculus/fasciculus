using Fasciculus.Eve.Models;
using Fasciculus.Support;
using Fasciculus.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEsiClient
    {
        public Task<EveMarketPrices?> GetMarketPricesAsync();

        public Task<EveRegionBuyOrders?> GetRegionBuyOrdersAsync(EveRegion region, IAccumulatingLongProgress progress);
        public Task<EveRegionSellOrders?> GetRegionSellOrdersAsync(EveRegion region, IAccumulatingLongProgress progress);
    }

    public partial class EsiClient : IEsiClient
    {
        private readonly IEsiCache esiCache;
        private readonly IEsiHttp esiHttp;

        private readonly EveTypes types;
        private readonly EveMoonStations stations;

        private readonly TaskSafeMutex mutex = new();

        public EsiClient(IEsiCache esiCache, IEsiHttp esiHttp, IEveResources resources)
        {
            this.esiCache = esiCache;
            this.esiHttp = esiHttp;

            types = Tasks.Wait(resources.Data).Types;
            stations = Tasks.Wait(resources.Universe).NpcStations;
        }
    }
}
