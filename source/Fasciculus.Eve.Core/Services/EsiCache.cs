using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Threading.Synchronization;
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

        public EveIndustryIndices.Data? GetIndustryIndices();
        public void SetIndustryIndices(EveIndustryIndices.Data data);
    }

    public class EsiCache : IEsiCache
    {
        public const int Version = 0;

#if DEBUG
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(360000);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(360000);
        public static readonly TimeSpan IndustryIndicesMaxAge = TimeSpan.FromSeconds(360000);
#else
        public static readonly TimeSpan MarketPricesMaxAge = TimeSpan.FromSeconds(3600);
        public static readonly TimeSpan MarketOrdersMaxAge = TimeSpan.FromSeconds(3600); // not using 600
        public static readonly TimeSpan IndustryIndicesMaxAge = TimeSpan.FromSeconds(3600);
#endif

        private readonly TaskSafeMutex mutex = new();

        private readonly DirectoryInfo marketDirectory;
        private readonly DirectoryInfo industryDirectory;

        public EsiCache(IEveFileSystem fileSystem)
        {
            marketDirectory = fileSystem.EsiCache.Combine("Market").CreateIfNotExists();
            industryDirectory = fileSystem.EsiCache.Combine("Industry").CreateIfNotExists();
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

        private FileInfo GetIndustryIndicesFile()
            => industryDirectory.File("IndustryIndices");

        public EveIndustryIndices.Data? GetIndustryIndices()
            => Read(GetIndustryIndicesFile(), IndustryIndicesMaxAge, s => new EveIndustryIndices.Data(s));

        public void SetIndustryIndices(EveIndustryIndices.Data data)
            => Write(GetIndustryIndicesFile(), data.Write);

        private T? Read<T>(FileInfo file, TimeSpan maxAge, Func<Binary, T> read)
            where T : notnull
        {
            using Locker locker = Locker.Lock(mutex);

            T? result = default;

            if (file.Exists && file.IsNewerThan(DateTime.UtcNow - maxAge))
            {
                using Stream stream = file.OpenRead();
                Binary bin = stream;
                int version = bin.ReadInt();

                if (version == Version)
                {
                    result = read(bin);
                }
            }

            return result;
        }

        private void Write(FileInfo file, Action<Binary> write)
        {
            using Locker locker = Locker.Lock(mutex);

            file.DeleteIfExists();

            using Stream stream = file.Create();
            Binary binary = stream;

            binary.WriteInt(Version);
            write(binary);
        }
    }
}
