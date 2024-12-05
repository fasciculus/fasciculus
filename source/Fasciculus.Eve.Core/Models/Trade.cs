using System;
using System.Collections.Generic;
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

    public class EveMarketPrices
    {
        public class Data
        {
            private readonly Dictionary<int, double> prices;
            public IReadOnlyDictionary<int, double> Prices => prices;

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

        public double this[EveType type]
            => data.Prices.TryGetValue(type.Id, out var price) ? price : 0;

        private readonly Lazy<EveType[]> tradedTypes;
        public IEnumerable<EveType> TradedTypes => tradedTypes.Value;

        public EveMarketPrices(Data data, EveTypes types)
        {
            this.data = data;

            tradedTypes = new(() => data.Prices.Keys.Select(id => types[id]).ToArray(), true);
        }
    }

    public class EveTrade
    {
    }
}