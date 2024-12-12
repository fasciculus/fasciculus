using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveMarketPrices
    {
        public class Data
        {
            private readonly Dictionary<int, double> prices;
            public IReadOnlyDictionary<int, double> Prices => prices;

            public Data(Dictionary<int, double> prices)
            {
                this.prices = prices.ToDictionary(x => x.Key, x => x.Value);
            }

            public Data(Stream stream)
            {
                prices = stream.ReadDictionary(s => s.ReadInt(), s => s.ReadDouble());
            }

            public void Write(Stream stream)
            {
                stream.WriteDictionary(prices, stream.WriteInt, stream.WriteDouble);
            }
        }

        private readonly Data data;

        public int Count => data.Prices.Count;

        public double this[EveType type]
            => data.Prices.TryGetValue(type.Id, out var price) ? price : 0;

        private readonly Lazy<EveType[]> tradedTypes;
        public IEnumerable<EveType> TradedTypes => tradedTypes.Value;

        public EveMarketPrices(Data data, EveTypes types)
        {
            this.data = data;

            tradedTypes = new(() => data.Prices.Keys
                .Where(x => types.Contains(x))
                .Select(id => types[id]).ToArray(), true);
        }

        public EveMarketPrices(Stream stream, EveTypes types)
            : this(new Data(stream), types) { }

        public void Write(Stream stream)
            => data.Write(stream);

        public static EveMarketPrices Empty(EveTypes types)
            => new(new Data([]), types);
    }
}
