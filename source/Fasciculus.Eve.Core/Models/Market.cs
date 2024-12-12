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

    public class EveMarketOrder
    {
        public class Data
        {
            public int Type { get; }
            public long Location { get; }
            public double Price { get; }
            public int Quantity { get; }

            public Data(int type, long location, double price, int quantity)
            {
                Type = type;
                Location = location;
                Price = price;
                Quantity = quantity;
            }

            public Data(Stream stream)
            {
                Type = stream.ReadInt();
                Location = stream.ReadLong();
                Price = stream.ReadDouble();
                Quantity = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Type);
                stream.WriteLong(Location);
                stream.WriteDouble(Price);
                stream.WriteInt(Quantity);
            }
        }

        internal readonly Data data;

        public int Type => data.Type;
        public long Location => data.Location;
        public double Price => data.Price;
        public int Quantity => data.Quantity;

        public EveMarketOrder(Data data)
        {
            this.data = data;
        }
    }

    public abstract class EveRegionOrders
    {
        public class Data
        {
            private readonly EveMarketOrder.Data[] orders;

            public Data(IEnumerable<EveMarketOrder.Data> orders)
            {
                this.orders = orders.ToArray();
            }

            public Data(Stream stream)
            {
                orders = stream.ReadArray(s => new EveMarketOrder.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteArray(orders, x => x.Write(stream));
            }
        }

        protected readonly Data data;

        protected readonly EveTypes types;
        protected readonly EveMoonStations stations;

        protected EveRegionOrders(Data data, EveTypes types, EveMoonStations stations)
        {
            this.data = data;
            this.types = types;
            this.stations = stations;
        }
    }

    public class EveRegionBuyOrders : EveRegionOrders
    {
        public EveRegionBuyOrders(Data data, EveTypes types, EveMoonStations stations)
            : base(data, types, stations)
        {
        }
    }

    public class EveRegionSellOrders : EveRegionOrders
    {
        public EveRegionSellOrders(Data data, EveTypes types, EveMoonStations stations)
            : base(data, types, stations)
        {

        }
    }
}
