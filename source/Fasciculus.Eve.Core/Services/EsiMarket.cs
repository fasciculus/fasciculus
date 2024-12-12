using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public partial class EsiClient : IEsiClient
    {
        public async Task<EveMarketPrices?> GetMarketPricesAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            EveMarketPrices? result = cache.MarketPrices;

            if (result is null)
            {
                string? text = await esiHttp.GetSingle("markets/prices/");

                if (text is not null)
                {
                    EsiMarketPrice[] esiMarketPrices = JsonSerializer.Deserialize<EsiMarketPrice[]>(text) ?? [];

                    if (esiMarketPrices.Length > 0)
                    {
                        Dictionary<int, double> prices = esiMarketPrices
                            .Where(x => x.AveragePrice > 0)
                            .ToDictionary(x => x.TypeId, x => x.AveragePrice);

                        EveMarketPrices.Data data = new(prices);
                        EveTypes types = (await resources.Data).Types;

                        result = new(data, types);
                        cache.MarketPrices = result;
                    }
                }
            }

            return result;
        }
    }
}
