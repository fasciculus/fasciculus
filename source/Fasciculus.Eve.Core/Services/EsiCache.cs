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

        private readonly EveTypes types;

        public EsiCache(IEveFileSystem fileSystem, IEveResources resources)
        {
            marketDirectory = fileSystem.EsiCache.Combine("Market").CreateIfNotExists();
            types = Tasks.Wait(resources.Data).Types;
        }

        public EveMarketPrices.Data? GetMarketPrices()
            => Read(marketDirectory.File("MarketPrices"), MarketPricesMaxAge, s => new EveMarketPrices.Data(s));

        public void SetMarketPrices(EveMarketPrices.Data data)
            => Write(marketDirectory.File("MarketPrices"), data.Write);

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
