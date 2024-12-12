namespace Fasciculus.Eve.Services
{
    public partial class EsiClient : IEsiClient
    {

        //public async Task<EveMarketOrders> GetMarketOrdersAsync(EveRegion region, bool buy)
        //{
        //    using Locker locker = Locker.Lock(mutex);

        //    EveMarketOrders? result = cache.GetMarketOrders(region, buy);

        //    if (result is null)
        //    {
        //        IEnumerable<EsiMarketOrder> esiOrders = [];

        //        string orderType = buy ? "buy" : "sell";
        //        int pageCount = 1;
        //        int page = 0;

        //        while (page < pageCount)
        //        {
        //            ++page;

        //            try
        //            {
        //                string uri = $"markets/{region.Id}/orders/?order_type={orderType}&page={page}";
        //                HttpRequestMessage request = new(HttpMethod.Get, uri);
        //                HttpResponseMessage response = await httpClient.SendAsync(request);

        //                response.EnsureSuccessStatusCode();

        //                string json = await response.Content.ReadAsStringAsync();
        //                EsiMarketOrder[] pageOrders = JsonSerializer.Deserialize<EsiMarketOrder[]>(json) ?? [];

        //                esiOrders = esiOrders.Concat(pageOrders);

        //                if (response.Headers.Contains("x-pages"))
        //                {
        //                    IEnumerable<string> values = response.Headers.GetValues("x-pages");
        //                    string value = values.FirstOrDefault() ?? "1";

        //                    if (int.TryParse(value, out int newPageCount))
        //                    {
        //                        pageCount = newPageCount;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.LogError("{message}", ex.Message);
        //                break;
        //            }
        //        }

        //        IEnumerable<EveMarketOrder.Data> orders = esiOrders.Select(Convert).NotNull();
        //        EveMarketOrders.Data data = new(orders);

        //        result = new(data);
        //        cache.SetMarketOrders(region, buy, result);
        //    }

        //    return result;
        //}

        //private EveMarketOrder.Data? Convert(EsiMarketOrder order)
        //{
        //    EveMarketOrder.Data? result = null;
        //    long location = order.LocationId;

        //    if (stations.Contains(location))
        //    {
        //        result = new(order.Type, location, order.IsBuyOrder, order.Price, order.Quantity);
        //    }

        //    return result;
        //}
    }
}
