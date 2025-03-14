using Fasciculus.IO.Binary;
using System;
using System.Collections;
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
                TypeId = stream.ReadInt32();
                AveragePrice = stream.ReadDouble();
                AdjustedPrice = stream.ReadDouble();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(TypeId);
                stream.WriteDouble(AveragePrice);
                stream.WriteDouble(AdjustedPrice);
            }
        }

        public EveType Type { get; }
        public double AveragePrice { get; }
        public double AdjustedPrice { get; }

        public EveMarketPrice(EveType type, double averagePrice, double adjustedPrice)
        {
            Type = type;
            AveragePrice = averagePrice;
            AdjustedPrice = adjustedPrice;
        }

        public EveMarketPrice(EveType type)
            : this(type, 0.0, 0.0) { }

        public EveMarketPrice(Data data, EveTypes types)
            : this(types[data.TypeId], data.AveragePrice, data.AdjustedPrice) { }
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
                stream.WriteArray(prices, (s, x) => x.Write(s));
            }
        }

        private readonly EveMarketPrice[] marketPrices;
        private readonly Dictionary<EveType, EveMarketPrice> byType;

        public IEnumerable<EveType> Types => byType.Keys;
        public bool Contains(EveType type) => byType.ContainsKey(type);
        public EveMarketPrice this[EveType type] => byType.TryGetValue(type, out EveMarketPrice? result) ? result : new(type);

        public EveMarketPrices(Data data, EveTypes types)
        {
            marketPrices = [.. data.Prices.Where(x => types.Contains(x.TypeId)).Select(x => new EveMarketPrice(x, types))];
            byType = marketPrices.ToDictionary(x => x.Type);
        }

        private EveMarketPrices()
        {
            marketPrices = [];
            byType = [];
        }

        public static readonly EveMarketPrices Empty = new();
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
                Type = stream.ReadInt32();
                Location = stream.ReadInt64();
                Price = stream.ReadDouble();
                Quantity = stream.ReadInt32();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Type);
                stream.WriteInt64(Location);
                stream.WriteDouble(Price);
                stream.WriteInt32(Quantity);
            }
        }

        internal readonly Data data;

        public double Price => data.Price;
        public int Quantity => data.Quantity;

        public EveType Type { get; }
        public EveStation Station { get; }
        public double Security { get; }

        public EveMarketOrder(Data data, EveTypes types, EveStations stations)
        {
            this.data = data;

            Type = types[data.Type];
            Station = stations[data.Location];
            Security = Station.Security;
        }
    }

    public abstract class EveMarketOrders : IEnumerable<EveMarketOrder>
    {
        private readonly EveMarketOrder[] orders;

        private readonly Lazy<Dictionary<EveType, EveMarketOrder[]>> byType;
        private readonly Lazy<Dictionary<EveStation, EveMarketOrder[]>> byStation;

        public int Count => orders.Length;
        public bool Contains(EveType type) => byType.Value.ContainsKey(type);

        protected EveMarketOrders(EveMarketOrder[] orders)
        {
            this.orders = orders;

            byType = new(FetchByType, true);
            byStation = new(FetchByStation, true);
        }

        public IEnumerator<EveMarketOrder> GetEnumerator()
            => orders.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => orders.GetEnumerator();

        protected EveMarketOrder[] OfType(EveType type)
            => byType.Value.TryGetValue(type, out EveMarketOrder[]? orders) ? orders : [];

        protected EveMarketOrder[] OfStation(EveStation station)
            => byStation.Value.TryGetValue(station, out EveMarketOrder[]? orders) ? orders : [];

        protected EveMarketOrder[] OfSecurity(EveSecurity.Level level)
        {
            EveSecurity.Filter filter = EveSecurity.Filters[level];

            return orders.Where(x => filter(x.Security)).ToArray();
        }

        private Dictionary<EveType, EveMarketOrder[]> FetchByType()
            => orders.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToArray());

        private Dictionary<EveStation, EveMarketOrder[]> FetchByStation()
            => orders.GroupBy(x => x.Station).ToDictionary(x => x.Key, x => x.ToArray());
    }

    public abstract class EveRegionOrders : EveMarketOrders
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
                stream.WriteArray(orders, (s, x) => x.Write(s));
            }
        }

        protected EveRegionOrders(Data data, EveTypes types, EveStations stations)
            : this(FetchOrders(data, types, stations)) { }

        protected EveRegionOrders(EveMarketOrder[] orders)
            : base(orders) { }

        private static EveMarketOrder[] FetchOrders(Data data, EveTypes types, EveStations stations)
        {
            return data.Orders
                .Where(x => types.Contains(x.Type))
                .Where(x => stations.Contains(x.Location))
                .Select(x => new EveMarketOrder(x, types, stations))
                .ToArray();
        }
    }

    public class EveRegionBuyOrders : EveRegionOrders
    {
        public EveTypeBuyOrders this[EveType type] => new(OfType(type), type);
        public EveStationBuyOrders this[EveStation station] => new(OfStation(station), station);
        public EveRegionBuyOrders this[EveSecurity.Level security] => new(OfSecurity(security));

        public EveRegionBuyOrders(Data data, EveTypes types, EveStations stations)
            : base(data, types, stations) { }

        private EveRegionBuyOrders(EveMarketOrder[] orders)
            : base(orders) { }

        public static readonly EveRegionBuyOrders Empty = new([]);
    }

    public class EveRegionSellOrders : EveRegionOrders
    {
        public EveTypeSellOrders this[EveType type] => new(OfType(type), type);
        public EveStationSellOrders this[EveStation station] => new(OfStation(station), station);
        public EveRegionSellOrders this[EveSecurity.Level security] => new(OfSecurity(security));

        public EveRegionSellOrders(Data data, EveTypes types, EveStations stations)
            : base(data, types, stations) { }

        private EveRegionSellOrders(EveMarketOrder[] orders)
            : base(orders) { }

        public static readonly EveRegionSellOrders Empty = new([]);
    }

    public class EveTypeBuyOrders : EveMarketOrders
    {
        private readonly EveType type;

        public EveDemand this[EveStation station]
            => new(type, station, OfStation(station));

        internal EveTypeBuyOrders(EveMarketOrder[] orders, EveType type)
            : base(orders)
        {
            this.type = type;
        }
    }

    public class EveTypeSellOrders : EveMarketOrders
    {
        private readonly EveType type;

        private readonly Lazy<Dictionary<EveStation, EveMarketOrder[]>> byStation;

        public EveSupply this[EveStation station]
            => new(type, station, OfStation(station));

        internal EveTypeSellOrders(EveMarketOrder[] orders, EveType type)
            : base(orders)
        {
            this.type = type;

            byStation = new(FetchByStation, true);
        }

        public double PriceFor(int quantity)
        {
            EveSupply[] supplies = byStation.Value.Keys
                .Select(x => this[x])
                .ToArray();

            if (supplies.Length > 0)
            {
                return supplies.Select(x => x.PriceFor(quantity)).Min();
            }

            return double.MaxValue;
        }

        private Dictionary<EveStation, EveMarketOrder[]> FetchByStation()
            => this.GroupBy(x => x.Station).ToDictionary(x => x.Key, x => x.ToArray());
    }

    public class EveStationBuyOrders : EveMarketOrders
    {
        private readonly EveStation station;

        public EveDemand this[EveType type]
            => new(type, station, OfType(type));

        internal EveStationBuyOrders(EveMarketOrder[] orders, EveStation station)
            : base(orders)
        {
            this.station = station;
        }
    }

    public class EveStationSellOrders : EveMarketOrders
    {
        private readonly EveStation station;

        public EveSupply this[EveType type]
            => new(type, station, OfType(type));

        internal EveStationSellOrders(EveMarketOrder[] orders, EveStation station)
            : base(orders)
        {
            this.station = station;
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
