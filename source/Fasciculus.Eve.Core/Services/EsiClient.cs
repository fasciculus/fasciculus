using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEsiClient
    {
        public Task<EveMarketPrices?> GetMarketPricesAsync();
    }

    public partial class EsiClient : IEsiClient
    {
        private readonly IEsiCache cache;
        private readonly IEsiHttp esiHttp;
        private readonly IEveResources resources;

        private readonly TaskSafeMutex mutex = new();

        public EsiClient(IEsiCache cache, IEsiHttp esiHttp, IEveResources resources)
        {
            this.cache = cache;
            this.esiHttp = esiHttp;
            this.resources = resources;
        }
    }
}
