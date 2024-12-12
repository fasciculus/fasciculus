using Fasciculus.Eve.Models;
using Fasciculus.Support;
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

            EveMarketPrices.Data? data = esiCache.GetMarketPrices();

            if (data is null)
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

                        data = new(prices);
                        esiCache.SetMarketPrices(data);
                    }
                }
            }

            return data is null ? null : new(data, types);
        }

        public async Task<EveRegionBuyOrders?> GetRegionBuyOrdersAsync(EveRegion region, IAccumulatingLongProgress progress)
        {
            using Locker locker = Locker.Lock(mutex);

            progress.Begin(1);

            EveRegionOrders.Data? data = esiCache.GetRegionBuyOrders(region);

            if (data is null)
            {
                data = await GetRegionOrders(region, "buy", progress);

                if (data is not null)
                {
                    esiCache.SetRegionBuyOrders(region, data);
                }
            }

            progress.End();

            return data is null ? null : new(data, types, stations);
        }

        public async Task<EveRegionSellOrders?> GetRegionSellOrdersAsync(EveRegion region, IAccumulatingLongProgress progress)
        {
            using Locker locker = Locker.Lock(mutex);

            progress.Begin(1);

            EveRegionOrders.Data? data = esiCache.GetRegionSellOrders(region);

            if (data is null)
            {
                data = await GetRegionOrders(region, "sell", progress);

                if (data is not null)
                {
                    esiCache.SetRegionSellOrders(region, data);
                }
            }

            progress.End();

            return data is null ? null : new(data, types, stations);
        }

        private async Task<EveRegionOrders.Data?> GetRegionOrders(EveRegion region, string orderType, IAccumulatingLongProgress progress)
        {
            string uri = $"markets/{region.Id}/orders/?order_type={orderType}";
            string[] texts = await esiHttp.GetPaged(uri, progress);
            EsiMarketOrder[] esiOrders = [.. texts.SelectMany(text => JsonSerializer.Deserialize<EsiMarketOrder[]>(text) ?? [])];

            if (esiOrders.Length > 0)
            {
                EveMarketOrder.Data[] orders = [.. esiOrders.Select(ConvertMarketOrder)];

                return new(orders);
            }

            return null;
        }

        private static EveMarketOrder.Data ConvertMarketOrder(EsiMarketOrder order)
            => new(order.Type, order.LocationId, order.Price, order.Quantity);
    }
}

