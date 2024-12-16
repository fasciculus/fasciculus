using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveMarketPrice
    {
        public class Data
        {
            public int TypeId { get; }
            public double AveragePrice { get; }
            public double AdjustedPrice { get; }

            public Data(int typeId, double averagePrice, double adjustedPrice)
            {
                TypeId = typeId;
                AveragePrice = averagePrice;
                AdjustedPrice = adjustedPrice;
            }

            public Data(Stream stream)
            {
                TypeId = stream.ReadInt();
                AveragePrice = stream.ReadDouble();
                AdjustedPrice = stream.ReadDouble();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(TypeId);
                stream.WriteDouble(AveragePrice);
                stream.WriteDouble(AdjustedPrice);
            }
        }

        private readonly Data data;
        private readonly EveTypes types;

        public EveType Type => types[data.TypeId];
        public double AveragePrice => data.AveragePrice;
        public double AdjustedPrice => data.AdjustedPrice;

        public EveMarketPrice(Data data, EveTypes types)
        {
            this.data = data;
            this.types = types;
        }
    }

    public class EveMarketPrices
    {
        public class Data
        {
            private EveMarketPrice.Data[] prices;

            public IReadOnlyList<EveMarketPrice.Data> Prices => prices;

            public Data(IEnumerable<EveMarketPrice.Data> prices)
            {
                this.prices = prices.ToArray();
            }

            public Data(Stream stream)
            {
                prices = stream.ReadArray(s => new EveMarketPrice.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteArray(prices, x => x.Write(stream));
            }
        }

        private readonly Data data;
        private readonly EveTypes types;

        private readonly Lazy<Dictionary<EveType, EveMarketPrice>> byType;

        public IEnumerable<EveType> Types => byType.Value.Keys;
        public bool Contains(EveType type) => byType.Value.ContainsKey(type);
        public EveMarketPrice this[EveType type] => byType.Value[type];

        public EveMarketPrices(Data data, EveTypes types)
        {
            this.data = data;
            this.types = types;

            byType = new(FetchByType, true);
        }

        private Dictionary<EveType, EveMarketPrice> FetchByType()
        {
            return data.Prices
                .Where(x => types.Contains(x.TypeId))
                .Select(x => Tuple.Create(types[x.TypeId], new EveMarketPrice(x, types)))
                .ToDictionary();
        }
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

            public int Count => orders.Length;

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
        protected readonly EveStations stations;

        private readonly Lazy<EveMarketOrder[]> orders;
        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;
        private readonly Lazy<Dictionary<EveStation, EveMarketOrder[]>> byStation;

        public int Count => data.Count;

        protected EveRegionOrders(Data data, EveTypes types, EveStations stations)
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

        protected EveMarketOrder[] GetOrders(EveStation station)
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

        private Dictionary<EveStation, EveMarketOrder[]> FetchByStation()
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
        public EveStationBuyOrders this[EveStation station] => new(station, types, GetOrders(station));

        public EveRegionBuyOrders(Data data, EveTypes types, EveStations stations)
            : base(data, types, stations) { }
    }

    public class EveRegionSellOrders : EveRegionOrders
    {
        public EveTypeSellOrders this[EveType type] => new(type, stations, GetOrders(type));
        public EveStationSellOrders this[EveStation station] => new(station, types, GetOrders(station));

        public EveRegionSellOrders(Data data, EveTypes types, EveStations stations)
            : base(data, types, stations) { }
    }

    public class EveTypeBuyOrders
    {
        private readonly EveType type;
        private readonly EveStations stations;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveStation, EveMarketOrder[]>> byStation;

        public int Count => orders.Length;

        public EveDemand this[EveStation station]
            => new(type, station, byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : []);

        internal EveTypeBuyOrders(EveType type, EveStations stations, EveMarketOrder[] orders)
        {
            this.type = type;
            this.stations = stations;
            this.orders = orders;

            byStation = new(FetchByStation, true);
        }

        private Dictionary<EveStation, EveMarketOrder[]> FetchByStation()
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
        private readonly EveStations stations;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveStation, EveMarketOrder[]>> byStation;

        public int Count => orders.Length;

        public EveSupply this[EveStation station]
            => new(type, station, byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : []);

        internal EveTypeSellOrders(EveType type, EveStations stations, EveMarketOrder[] orders)
        {
            this.type = type;
            this.stations = stations;
            this.orders = orders;

            byStation = new(FetchByStation, true);
        }

        public double PriceFor(int quantity)
        {
            EveSupply[] supplies = byStation.Value.Keys
                .Where(x => x.Moon.Planet.SolarSystem.Security >= 0.5)
                .Select(x => this[x])
                .ToArray();

            if (supplies.Length > 0)
            {
                return supplies.Select(x => x.PriceFor(quantity)).Min();
            }

            return double.MaxValue;
        }

        private Dictionary<EveStation, EveMarketOrder[]> FetchByStation()
        {
            return orders.GroupBy(x => x.Location)
                .Where(x => stations.Contains(x.Key))
                .Select(x => Tuple.Create(stations[x.Key], x.ToArray()))
                .ToDictionary();
        }
    }

    public class EveStationBuyOrders
    {
        private readonly EveStation station;
        private readonly EveTypes types;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;

        public EveDemand this[EveType type]
            => new(type, station, byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : []);

        internal EveStationBuyOrders(EveStation station, EveTypes types, EveMarketOrder[] orders)
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
        private readonly EveStation station;
        private readonly EveTypes types;
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;

        public EveSupply this[EveType type]
            => new(type, station, byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : []);

        internal EveStationSellOrders(EveStation station, EveTypes types, EveMarketOrder[] orders)
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
        public EveStation Station { get; }

        private readonly EveMarketOrder[] orders;

        internal EveDemand(EveType type, EveStation station, EveMarketOrder[] orders)
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
        public EveStation Station { get; }

        private readonly EveMarketOrder[] orders;

        internal EveSupply(EveType type, EveStation station, EveMarketOrder[] orders)
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
