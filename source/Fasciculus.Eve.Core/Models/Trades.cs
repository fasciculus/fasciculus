using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveTradeOptions : IEquatable<EveTradeOptions>
    {
        public const long DefaultTargetStationId = 60003760;
        public const int DefaultMaxDistance = 5;
        public const int DefaultMaxVolumePerType = 5000;
        public const int DefaultMaxIskPerType = 10_000_000;

        public class Data : IEquatable<Data>
        {
            public int MaxDistance { get; }
            public int MaxVolumePerType { get; }
            public int MaxIskPerType { get; }

            public Data(int maxDistance, int maxVolumePerType, int maxIskPerType)
            {
                MaxDistance = maxDistance;
                MaxVolumePerType = maxVolumePerType;
                MaxIskPerType = maxIskPerType;
            }

            public Data()
                : this(DefaultMaxDistance, DefaultMaxVolumePerType, DefaultMaxIskPerType) { }

            public Data(Data data)
                : this(data.MaxDistance, data.MaxVolumePerType, data.MaxIskPerType) { }

            public Data(Stream stream)
            {
                MaxDistance = stream.ReadInt();
                MaxVolumePerType = stream.ReadInt();
                MaxIskPerType = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(MaxDistance);
                stream.WriteInt(MaxVolumePerType);
                stream.WriteInt(MaxIskPerType);
            }

            public bool Equals(Data? other)
            {
                if (other is null) return false;

                return MaxDistance == other.MaxDistance
                    && MaxVolumePerType == other.MaxVolumePerType
                    && MaxIskPerType == other.MaxIskPerType;
            }

            public override bool Equals(object? obj)
                => obj is Data data && Equals(data);

            public override int GetHashCode()
                => MaxDistance.GetHashCode() ^ MaxVolumePerType.GetHashCode() ^ MaxIskPerType.GetHashCode();
        }

        private readonly EveMoonStations stations;
        private readonly Data data;

        public EveMoonStation TargetStation { get; }
        public int MaxDistance => data.MaxDistance;
        public int MaxVolumePerType => data.MaxVolumePerType;
        public int MaxIskPerType => data.MaxIskPerType;

        public EveTradeOptions(Data data, EveMoonStations stations)
        {
            this.stations = stations;
            this.data = new(data);

            TargetStation = stations[DefaultTargetStationId];
        }

        public EveTradeOptions(EveMoonStations stations)
            : this(new Data(), stations) { }

        public EveTradeOptions(EveTradeOptions options)
            : this(options.data, options.stations) { }

        public EveTradeOptions(Stream stream, EveMoonStations stations)
            : this(new Data(stream), stations) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }

        public bool Equals(EveTradeOptions? other)
        {
            if (other is null) return false;

            return data.Equals(other.data);
        }

        public override bool Equals(object? obj)
            => obj is EveTradeOptions other && Equals(other);

        public override int GetHashCode()
            => data.GetHashCode();
    }

    public class EveMarketOrder
    {
        public class Data
        {
            public int Type { get; }
            public long Location { get; }
            public bool IsBuy { get; }
            public double Price { get; }
            public int Quantity { get; }

            public Data(int type, long location, bool isBuy, double price, int quantity)
            {
                Type = type;
                Location = location;
                IsBuy = isBuy;
                Price = price;
                Quantity = quantity;
            }

            public Data(Stream stream)
            {
                Type = stream.ReadInt();
                Location = stream.ReadLong();
                IsBuy = stream.ReadBool();
                Price = stream.ReadDouble();
                Quantity = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Type);
                stream.WriteLong(Location);
                stream.WriteBool(IsBuy);
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

    public class EveMarketOrders : IEnumerable<EveMarketOrder>
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

        private readonly Data data;

        private readonly EveMarketOrder[] orders;

        public EveMarketOrders(Data data)
        {
            this.data = data;

            orders = data.Orders.Select(x => new EveMarketOrder(x)).ToArray();
        }

        public EveMarketOrders(IEnumerable<EveMarketOrder> orders)
        {
            this.orders = orders.ToArray();

            data = new(orders.Select(x => x.data));
        }

        public EveMarketOrders(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }

        public static EveMarketOrders Empty
            => new(new Data([]));

        public IEnumerator<EveMarketOrder> GetEnumerator()
            => orders.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => orders.GetEnumerator();
    }

    public class EveDemandOrSupply
    {
        public EveMoonStation Station { get; }
        public EveType Type { get; }
        public double Price { get; }
        public int Quantity { get; }

        public EveDemandOrSupply(EveMoonStation station, EveType type, double price, int quantity)
        {
            Station = station;
            Type = type;
            Price = price;
            Quantity = quantity;
        }
    }

    [DebuggerDisplay("{ProfitText}")]
    public class EveTrade
    {
        public EveDemandOrSupply Supply { get; }
        public EveDemandOrSupply Demand { get; }
        public int Quantity { get; }
        public double Profit { get; }

        public string QuantityText => Quantity.ToString("#,###,###,##0");
        public string BuyPriceText => Supply.Price.ToString("#,###,###,##0.00");
        public string SellPriceText => Demand.Price.ToString("#,###,###,##0.00");
        public string ProfitText => Profit.ToString("#,###,###,##0.00");

        public EveTrade(EveDemandOrSupply supply, EveDemandOrSupply demand, int quantity, double profit)
        {
            Supply = supply;
            Demand = demand;
            Quantity = quantity;
            Profit = profit;
        }
    }
}