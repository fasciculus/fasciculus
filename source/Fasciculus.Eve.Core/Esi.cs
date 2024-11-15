using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Fasciculus.Eve
{
    public class EsiCacheEntry
    {
        public DateTime Expires { get; set; }

        public bool IsExpired
            => DateTime.UtcNow > Expires;

        public EsiCacheEntry(DateTime expires)
        {
            Expires = expires;
        }
    }

    public class EsiCacheEntry<T> : EsiCacheEntry
        where T : notnull
    {
        public T Value { get; set; }

        public EsiCacheEntry(DateTime expires, T value)
            : base(expires)
        {
            Value = value;
        }
    }

    public class EsiCache
    {
        private readonly FileInfo file;
        private readonly EsiCacheEntry<EsiMarketPrice[]> marketPrices = new(DateTime.UtcNow, []);

        private Dictionary<int, Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>>> marketOrders = [];

        public EsiCache(FileInfo file)
        {
            this.file = file;
        }

        public EsiMarketPrice[]? GetMarketPrices()
        {
            return marketPrices.IsExpired ? null : marketPrices.Value;
        }

        public EsiMarketPrice[] SetMarketPrices(EsiMarketPrice[]? marketPrices, int duration)
        {
            if (marketPrices is null) return [];

            this.marketPrices.Expires = DateTime.UtcNow.AddSeconds(duration);
            this.marketPrices.Value = marketPrices;
            Write();

            return marketPrices;
        }

        public EsiMarketOrder[]? GetMarketOrders(int region, int type)
        {
            EsiCacheEntry<EsiMarketOrder[]>? entry = null;

            marketOrders.TryGetValue(region, out Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>>? regionOrders);
            regionOrders?.TryGetValue(type, out entry);

            return (entry is null || entry.IsExpired) ? null : entry.Value;
        }

        public EsiMarketOrder[] SetMarketOrders(int region, int type, EsiMarketOrder[]? orders, int duration)
        {
            if (orders is null) return [];

            marketOrders.TryGetValue(region, out Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>>? regionOrders);

            if (regionOrders is null)
            {
                marketOrders[region] = regionOrders = [];
            }

            regionOrders.TryGetValue(type, out EsiCacheEntry<EsiMarketOrder[]>? entry);

            DateTime expires = DateTime.UtcNow.AddSeconds(duration);

            if (entry is null)
            {
                regionOrders[type] = new(expires, orders);
            }
            else
            {
                entry.Expires = expires;
                entry.Value = orders;
            }

            Write();

            return orders;
        }

        public static EsiCache Read(FileInfo file)
        {
            EsiCache cache = new(file);

            if (file.Exists)
            {
                file.Read(s => Read(s, cache));
            }

            return cache;
        }

        private void Write()
        {
            file.Write(s => Write(s));
        }

        private static void Read(Stream stream, EsiCache cache)
        {
            cache.marketPrices.Expires = stream.ReadDateTime();
            cache.marketPrices.Value = stream.ReadArray(EsiMarketPrice.Read);

            cache.marketOrders = ReadMarketOrders(stream);
        }

        private static Dictionary<int, Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>>> ReadMarketOrders(Stream stream)
            => stream.ReadDictionary(s => s.ReadInt(), ReadRegionOrders);

        private static Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>> ReadRegionOrders(Stream stream)
            => stream.ReadDictionary(s => s.ReadInt(), ReadRegionOrdersEntry);

        private static EsiCacheEntry<EsiMarketOrder[]> ReadRegionOrdersEntry(Stream stream)
        {
            DateTime expires = stream.ReadDateTime();
            EsiMarketOrder[] orders = stream.ReadArray(EsiMarketOrder.Read);

            return new(expires, orders);
        }

        private void Write(Stream stream)
        {
            stream.WriteDateTime(marketPrices.Expires);
            stream.WriteArray(marketPrices.Value, mp => mp.Write(stream));

            WriteMarketOrders(stream, marketOrders);
        }

        private static void WriteMarketOrders(Stream stream, Dictionary<int, Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>>> orders)
            => stream.WriteDictionary(orders, k => stream.WriteInt(k), v => WriteRegionOrders(stream, v));

        private static void WriteRegionOrders(Stream stream, Dictionary<int, EsiCacheEntry<EsiMarketOrder[]>> entries)
            => stream.WriteDictionary(entries, k => stream.WriteInt(k), v => WriteRegionOrdersEntry(stream, v));

        private static void WriteRegionOrdersEntry(Stream stream, EsiCacheEntry<EsiMarketOrder[]> entry)
        {
            stream.WriteDateTime(entry.Expires);
            stream.WriteArray(entry.Value, mp => mp.Write(stream));
        }
    }

    public class Esi : IDisposable
    {
        private readonly HttpClient client;
        private readonly EsiCache cache;

        public Esi(string userAgent, FileInfo cacheFile)
        {
            client = CreateClient(userAgent);
            cache = EsiCache.Read(cacheFile);
        }

        ~Esi()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            client.Dispose();
        }

        public async Task<EsiMarketPrice[]> GetMarketPricesAsync()
        {
            EsiMarketPrice[]? result = cache.GetMarketPrices();

            if (result is null)
            {
                result = await client.GetFromJsonAsync<EsiMarketPrice[]>("markets/prices/?datasource=tranquility");
                result = cache.SetMarketPrices(result, 3600);
            }

            return result;
        }

        public async Task<EsiMarketOrder[]> GetMarketOrdersAsync(int region, int type)
        {
            EsiMarketOrder[]? result = cache.GetMarketOrders(region, type);

            if (result is null)
            {
                string uri = $"markets/{region}/orders/?datasource=tranquility&order_type=all&page=1&type_id={type}";

                result = await client.GetFromJsonAsync<EsiMarketOrder[]>(uri);
                result = cache.SetMarketOrders(region, type, result, 300);
            }

            return result;
        }

        private static HttpClient CreateClient(string userAgent)
        {
            HttpClient client = new(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            })
            {
                BaseAddress = new Uri("https://esi.evetech.net/latest/")
            };

            client.DefaultRequestHeaders.Add("X-User-Agent", userAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return client;
        }
    }
}
