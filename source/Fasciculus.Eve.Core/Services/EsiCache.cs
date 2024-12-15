using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using System;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public interface IEsiCache
    {
        public EveMarketPrices.Data? GetMarketPrices();
        public void SetMarketPrices(EveMarketPrices.Data data);

        public EveRegionOrders.Data? GetRegionBuyOrders(EveRegion region);
        public void SetRegionBuyOrders(EveRegion region, EveRegionOrders.Data data);

        public EveRegionOrders.Data? GetRegionSellOrders(EveRegion region);
        public void SetRegionSellOrders(EveRegion region, EveRegionOrders.Data data);
    }

    public class EsiCache : IEsiCache
    {
#if DEBUG
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(360000);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(360000);
#else
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(3600);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(300);
#endif

        private readonly TaskSafeMutex mutex = new();

        private readonly DirectoryInfo marketDirectory;

        public EsiCache(IEveFileSystem fileSystem)
        {
            marketDirectory = fileSystem.EsiCache.Combine("Market").CreateIfNotExists();
        }

        private FileInfo GetMarketPricesFile()
            => marketDirectory.File("MarketPrices");

        public EveMarketPrices.Data? GetMarketPrices()
            => Read(GetMarketPricesFile(), MarketPricesMaxAge, s => new EveMarketPrices.Data(s));

        public void SetMarketPrices(EveMarketPrices.Data data)
            => Write(GetMarketPricesFile(), data.Write);

        private FileInfo GetRegionBuyOrdersFile(EveRegion region)
            => marketDirectory.File($"{region.Id}_b");

        public EveRegionOrders.Data? GetRegionBuyOrders(EveRegion region)
            => Read(GetRegionBuyOrdersFile(region), MarketOrdersMaxAge, s => new EveRegionOrders.Data(s));

        public void SetRegionBuyOrders(EveRegion region, EveRegionOrders.Data data)
            => Write(GetRegionBuyOrdersFile(region), data.Write);

        private FileInfo GetRegionSellOrdersFile(EveRegion region)
            => marketDirectory.File($"{region.Id}_s");

        public EveRegionOrders.Data? GetRegionSellOrders(EveRegion region)
            => Read(GetRegionSellOrdersFile(region), MarketOrdersMaxAge, s => new EveRegionOrders.Data(s));

        public void SetRegionSellOrders(EveRegion region, EveRegionOrders.Data data)
            => Write(GetRegionSellOrdersFile(region), data.Write);

        private T? Read<T>(FileInfo file, TimeSpan maxAge, Func<Stream, T> read)
            where T : notnull
        {
            using Locker locker = Locker.Lock(mutex);

            T? result = default;

            if (file.Exists && file.IsNewerThan(DateTime.UtcNow - maxAge))
            {
                result = file.Read(read);
            }

            return result;
        }

        private void Write(FileInfo file, Action<Stream> write)
        {
            using Locker locker = Locker.Lock(mutex);

            file.Write(write);
        }
    }
}
