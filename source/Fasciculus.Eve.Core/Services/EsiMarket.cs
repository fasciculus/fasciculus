using Fasciculus.Eve.Models;
using Fasciculus.Progress;
using Fasciculus.Threading.Synchronization;
using System;
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
                        EveMarketPrice.Data[] prices = [.. esiMarketPrices
                            .Select(ConvertMarketPrice)
                            .Where(x => types.Contains(x.TypeId))];

                        data = new(prices);
                        esiCache.SetMarketPrices(data);
                    }
                }
            }

            return data is null ? null : new(data, types);
        }

        private static EveMarketPrice.Data ConvertMarketPrice(EsiMarketPrice price)
            => new(price.TypeId, price.AveragePrice, price.AdjustedPrice);

        public async Task<EveRegionBuyOrders?> GetRegionBuyOrdersAsync(EveRegion region, IAccumulatingProgress<long> progress)
        {
            using Locker locker = Locker.Lock(mutex);

            progress.Begin(1);

            EveRegionOrders.Data? data = esiCache.GetRegionBuyOrders(region);

            if (data is null)
            {
                Tuple<EveRegionOrders.Data?, bool> response = await GetRegionOrders(region, "buy", progress);

                data = response.Item1;

                if (data is not null && response.Item2)
                {
                    esiCache.SetRegionBuyOrders(region, data);
                }
            }

            progress.End();

            return data is null ? null : new(data, types, stations);
        }

        public async Task<EveRegionSellOrders?> GetRegionSellOrdersAsync(EveRegion region, IAccumulatingProgress<long> progress)
        {
            using Locker locker = Locker.Lock(mutex);

            progress.Begin(1);

            EveRegionOrders.Data? data = esiCache.GetRegionSellOrders(region);

            if (data is null)
            {
                Tuple<EveRegionOrders.Data?, bool> response = await GetRegionOrders(region, "sell", progress);

                data = response.Item1;

                if (data is not null && response.Item2)
                {
                    esiCache.SetRegionSellOrders(region, data);
                }
            }

            progress.End();

            return data is null ? null : new(data, types, stations);
        }

        private async Task<Tuple<EveRegionOrders.Data?, bool>> GetRegionOrders(EveRegion region, string orderType, IAccumulatingProgress<long> progress)
        {
            string uri = $"markets/{region.Id}/orders/?order_type={orderType}";
            Tuple<string[], bool> response = await esiHttp.GetPaged(uri, progress);
            EsiMarketOrder[] esiOrders = [.. response.Item1.SelectMany(text => JsonSerializer.Deserialize<EsiMarketOrder[]>(text) ?? [])];

            if (esiOrders.Length > 0)
            {
                EveMarketOrder.Data[] orders = [.. esiOrders
                    .Select(ConvertMarketOrder)
                    .Where(x => stations.Contains(x.Location) && types.Contains(x.Type))];

                EveRegionOrders.Data data = new(orders);
                Tuple<EveRegionOrders.Data?, bool> result = new(data, response.Item2);

                return result;
            }

            return new Tuple<EveRegionOrders.Data?, bool>(null, false);
        }

        private static EveMarketOrder.Data ConvertMarketOrder(EsiMarketOrder order)
            => new(order.Type, order.LocationId, order.Price, order.Quantity);
    }
}

