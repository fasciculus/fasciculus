using Fasciculus.Eve.Models;
using Fasciculus.Models;
using Fasciculus.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
        private class MarketOrderKey : ComparablePair<int, int>
        {
            public int Region => First;
            public int Type => Second;

            public MarketOrderKey(int region, int type)
                : base(region, type) { }

            public static MarketOrderKey Read(Stream stream)
            {
                int region = stream.ReadInt();
                int type = stream.ReadInt();

                return new(region, type);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Region);
                stream.WriteInt(Type);
            }
        }

        private readonly TaskSafeMutex mutex = new();

        private readonly FileInfo file;
        private bool dirty = false;

        private readonly EsiCacheEntry<EsiMarketPrice[]> marketPrices = new(DateTime.UtcNow, []);
        private Dictionary<MarketOrderKey, EsiCacheEntry<EsiMarketOrder[]>> marketOrders = [];

        public EsiCache(FileInfo file)
        {
            this.file = file;
        }

        public EsiMarketPrice[]? GetMarketPrices()
        {
            using Locker locker = Locker.Lock(mutex);

            return marketPrices.IsExpired ? null : marketPrices.Value;
        }

        public EsiMarketPrice[] SetMarketPrices(EsiMarketPrice[]? marketPrices, int duration)
        {
            using Locker locker = Locker.Lock(mutex);

            if (marketPrices is null) return [];

            this.marketPrices.Expires = DateTime.UtcNow.AddSeconds(duration);
            this.marketPrices.Value = marketPrices;

            dirty = true;

            return marketPrices;
        }

        public EsiMarketOrder[]? GetMarketOrders(int region, int type)
        {
            using Locker locker = Locker.Lock(mutex);
            MarketOrderKey key = new(region, type);

            marketOrders.TryGetValue(key, out EsiCacheEntry<EsiMarketOrder[]>? entry);

            return (entry is null || entry.IsExpired) ? null : entry.Value;
        }

        public EsiMarketOrder[] SetMarketOrders(int region, int type, EsiMarketOrder[]? orders, int duration)
        {
            using Locker locker = Locker.Lock(mutex);

            if (orders is null) return [];

            MarketOrderKey key = new(region, type);

            marketOrders.TryGetValue(key, out EsiCacheEntry<EsiMarketOrder[]>? entry);

            DateTime expires = DateTime.UtcNow.AddSeconds(duration);

            if (entry is null)
            {
                marketOrders[key] = new(expires, orders);
            }
            else
            {
                entry.Expires = expires;
                entry.Value = orders;
            }

            dirty = true;

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

        public void Flush()
        {
            using Locker locker = Locker.Lock(mutex);

            if (dirty)
            {
                file.Write(Write);
                dirty = false;
            }
        }

        private static void Read(Stream stream, EsiCache cache)
        {
            cache.marketPrices.Expires = stream.ReadDateTime();
            cache.marketPrices.Value = stream.ReadArray(EsiMarketPrice.Read);

            cache.marketOrders = stream.ReadDictionary(MarketOrderKey.Read, ReadMarketOrderEntry);
        }

        private void Write(Stream stream)
        {
            stream.WriteDateTime(marketPrices.Expires);
            stream.WriteArray(marketPrices.Value, mp => mp.Write(stream));

            stream.WriteDictionary(marketOrders, k => k.Write(stream), v => WriteMarketOrderEntry(stream, v));
        }

        private static EsiCacheEntry<EsiMarketOrder[]> ReadMarketOrderEntry(Stream stream)
        {
            DateTime expires = stream.ReadDateTime();
            EsiMarketOrder[] orders = stream.ReadArray(EsiMarketOrder.Read);

            return new(expires, orders);
        }

        private static void WriteMarketOrderEntry(Stream stream, EsiCacheEntry<EsiMarketOrder[]> entry)
        {
            stream.WriteDateTime(entry.Expires);
            stream.WriteArray(entry.Value, v => v.Write(stream));
        }
    }

    public class Esi : IDisposable
    {
        public class EsiErrorEventArgs : EventArgs
        {
            public string Uri { get; }
            public HttpStatusCode StatusCode { get; }

            public EsiErrorEventArgs(string uri, HttpStatusCode statusCode)
            {
                Uri = uri;
                StatusCode = statusCode;
            }
        }

        private readonly HttpClient client;
        private readonly EsiCache cache;

        public event EventHandler<EsiErrorEventArgs>? OnError;

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

        public void Flush()
        {
            cache.Flush();
        }

        public async Task<EsiMarketPrice[]> GetMarketPricesAsync()
        {
            string uri = "markets/prices/?datasource=tranquility";
            using Locker locker = NamedTaskSafeMutexes.Lock(uri);

            EsiMarketPrice[]? result = cache.GetMarketPrices();

            if (result is null)
            {
                result = await Get(uri, s => JsonSerializer.Deserialize<EsiMarketPrice[]>(s));
                result = cache.SetMarketPrices(result, 3600);
            }

            return result;
        }

        public async Task<EsiMarketOrder[]> GetMarketOrdersAsync(int region, int type)
        {
            string uri = $"markets/{region}/orders/?datasource=tranquility&order_type=all&page=1&type_id={type}";
            using Locker locker = NamedTaskSafeMutexes.Lock(uri);

            EsiMarketOrder[]? result = cache.GetMarketOrders(region, type);

            if (result is null)
            {
                result = await Get(uri, s => JsonSerializer.Deserialize<EsiMarketOrder[]>(s));
                result = cache.SetMarketOrders(region, type, result, 300);
            }

            return result;
        }

        private async Task<T?> Get<T>(string uri, Func<string, T> deserialize)
        {
            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                OnError?.Invoke(this, new(uri, response.StatusCode));
                return default;
            }

            string serialized = await response.Content.ReadAsStringAsync();

            return deserialize(serialized);
        }

        private static HttpClient CreateClient(string userAgent)
        {
            HttpClient client = new(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = 32,
            });

            client.BaseAddress = new Uri("https://esi.evetech.net/latest/");

            client.DefaultRequestHeaders.Add("X-User-Agent", userAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            return client;
        }
    }
}
