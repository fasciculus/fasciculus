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
            public IReadOnlyList<EveMarketOrder.Data> Orders => orders;

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

        private readonly Lazy<EveMarketOrder[]> orders;
        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;
        private readonly Lazy<Dictionary<EveMoonStation, EveMarketOrder[]>> byStation;

        protected EveRegionOrders(Data data, EveTypes types, EveMoonStations stations)
        {
            this.data = data;
            this.types = types;
            this.stations = stations;

            orders = new(FetchOrders, true);
            byType = new(FetchByType, true);
            byStation = new(FetchByStation, true);
        }

        protected EveMarketOrder[] GetOrders(EveType type)
            => byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : [];

        protected EveMarketOrder[] GetOrders(EveMoonStation station)
            => byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : [];

        private EveMarketOrder[] FetchOrders()
            => [.. data.Orders.Select(x => new EveMarketOrder(x))];

        private Dictionary<EveType, EveMarketOrder[]> FetchByType()
        {
            return orders.Value.GroupBy(x => x.Type)
                .Where(x => types.Contains(x.Key))
                .Select(x => Tuple.Create(types[x.Key], x.ToArray()))
                .ToDictionary();
        }

        private Dictionary<EveMoonStation, EveMarketOrder[]> FetchByStation()
        {
            return orders.Value.GroupBy(x => x.Location)
                .Where(x => stations.Contains(x.Key))
                .Select(x => Tuple.Create(stations[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveRegionBuyOrders : EveRegionOrders
    {
        public EveTypeBuyOrders this[EveType type] => new(type, stations, GetOrders(type));
        public EveStationBuyOrders this[EveMoonStation station] => new(station, types, GetOrders(station));

        public EveRegionBuyOrders(Data data, EveTypes types, EveMoonStations stations)
            : base(data, types, stations) { }
    }

    public class EveRegionSellOrders : EveRegionOrders
    {
        public EveTypeSellOrders this[EveType type] => new(type, stations, GetOrders(type));
        public EveStationSellOrders this[EveMoonStation station] => new(station, types, GetOrders(station));

        public EveRegionSellOrders(Data data, EveTypes types, EveMoonStations stations)
            : base(data, types, stations) { }
    }

    public class EveTypeBuyOrders
    {
        private readonly EveType type;
        private readonly EveMoonStations stations;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveMoonStation, EveMarketOrder[]>> byStation;

        public EveDemand this[EveMoonStation station]
            => new(type, station, byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : []);

        internal EveTypeBuyOrders(EveType type, EveMoonStations stations, EveMarketOrder[] orders)
        {
            this.type = type;
            this.stations = stations;
            this.orders = orders;

            byStation = new(FetchByStation, true);
        }

        private Dictionary<EveMoonStation, EveMarketOrder[]> FetchByStation()
        {
            return orders.GroupBy(x => x.Location)
                .Where(x => stations.Contains(x.Key))
                .Select(x => Tuple.Create(stations[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveTypeSellOrders
    {
        private readonly EveType type;
        private readonly EveMoonStations stations;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveMoonStation, EveMarketOrder[]>> byStation;

        public EveSupply this[EveMoonStation station]
            => new(type, station, byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : []);

        internal EveTypeSellOrders(EveType type, EveMoonStations stations, EveMarketOrder[] orders)
        {
            this.type = type;
            this.stations = stations;
            this.orders = orders;

            byStation = new(FetchByStation, true);
        }

        private Dictionary<EveMoonStation, EveMarketOrder[]> FetchByStation()
        {
            return orders.GroupBy(x => x.Location)
                .Where(x => stations.Contains(x.Key))
                .Select(x => Tuple.Create(stations[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveStationBuyOrders
    {
        private readonly EveMoonStation station;
        private readonly EveTypes types;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;

        public EveDemand this[EveType type]
            => new(type, station, byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : []);

        internal EveStationBuyOrders(EveMoonStation station, EveTypes types, EveMarketOrder[] orders)
        {
            this.station = station;
            this.types = types;
            this.orders = orders;

            byType = new(FetchByType, true);
        }

        private Dictionary<EveType, EveMarketOrder[]> FetchByType()
        {
            return orders.GroupBy(x => x.Type)
                .Where(x => types.Contains(x.Key))
                .Select(x => Tuple.Create(types[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveStationSellOrders
    {
        private readonly EveMoonStation station;
        private readonly EveTypes types;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;

        public EveSupply this[EveType type]
            => new(type, station, byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : []);

        internal EveStationSellOrders(EveMoonStation station, EveTypes types, EveMarketOrder[] orders)
        {
            this.station = station;
            this.types = types;
            this.orders = orders;

            byType = new(FetchByType, true);
        }

        private Dictionary<EveType, EveMarketOrder[]> FetchByType()
        {
            return orders.GroupBy(x => x.Type)
                .Where(x => types.Contains(x.Key))
                .Select(x => Tuple.Create(types[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveDemand
    {
        public EveType Type { get; }
        public EveMoonStation Station { get; }

        private readonly EveMarketOrder[] orders;

        internal EveDemand(EveType type, EveMoonStation station, EveMarketOrder[] orders)
        {
            Type = type;
            Station = station;

            this.orders = [.. orders.OrderByDescending(x => x.Price)];
        }

        public double PriceFor(int quantity)
        {
            double price = 0;

            foreach (EveMarketOrder order in orders)
            {
                price = order.Price;
                quantity -= order.Quantity;

                if (quantity <= 0)
                {
                    break;
                }
            }

            return price;
        }
    }

    public class EveSupply
    {
        public EveType Type { get; }
        public EveMoonStation Station { get; }

        private readonly EveMarketOrder[] orders;

        internal EveSupply(EveType type, EveMoonStation station, EveMarketOrder[] orders)
        {
            Type = type;
            Station = station;

            this.orders = [.. orders.OrderBy(x => x.Price)];
        }

        public double PriceFor(int quantity)
        {
            double price = double.MaxValue;

            foreach (EveMarketOrder order in orders)
            {
                price = order.Price;
                quantity -= order.Quantity;

                if (quantity <= 0)
                {
                    break;
                }
            }

            return price;
        }
    }
}
