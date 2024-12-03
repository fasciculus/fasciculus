using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveType
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;
            public double Volume { get; }

            public Data(int id, string name, double volume)
            {
                Id = id;
                Name = name;
                Volume = volume;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Volume = stream.ReadDouble();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Volume);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;
        public double Volume => data.Volume;

        public EveType(Data data)
        {
            this.data = data;
        }
    }

    public class EveTypes : IEnumerable<EveType>
    {
        private Dictionary<int, EveType> byId;

        public EveType this[int id] => byId[id];

        public EveTypes(IEnumerable<EveType> types)
        {
            byId = types.ToDictionary(x => x.Id);
        }

        public IEnumerator<EveType> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveStationOperation
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;

            public Data(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveStationOperation(Data data)
        {
            this.data = data;
        }
    }

    public class EveStationOperations : IEnumerable<EveStationOperation>
    {
        private Dictionary<int, EveStationOperation> byId;

        public EveStationOperation this[int id] => byId[id];

        public EveStationOperations(IEnumerable<EveStationOperation> operations)
        {
            byId = operations.ToDictionary(x => x.Id);
        }

        public IEnumerator<EveStationOperation> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveNpcCorporation
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;

            public Data(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveNpcCorporation(Data data)
        {
            this.data = data;
        }
    }

    public class EveNpcCorporations : IEnumerable<EveNpcCorporation>
    {
        private Dictionary<int, EveNpcCorporation> byId;

        public EveNpcCorporation this[int id] => byId[id];

        public EveNpcCorporations(IEnumerable<EveNpcCorporation> operations)
        {
            byId = operations.ToDictionary(x => x.Id);
        }

        public IEnumerator<EveNpcCorporation> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveData
    {
        public class Data
        {
            public DateTime Version { get; }
            public EveType.Data[] Types { get; }
            public EveStationOperation.Data[] StationOperations { get; }
            public EveNpcCorporation.Data[] NpcCorporations { get; }

            public Data(DateTime version, EveType.Data[] types, EveStationOperation.Data[] stationOperations,
                EveNpcCorporation.Data[] npcCorporations)
            {
                Version = version;
                Types = types;
                StationOperations = stationOperations;
                NpcCorporations = npcCorporations;
            }

            public Data(Stream stream)
            {
                Version = stream.ReadDateTime();
                Types = stream.ReadArray(s => new EveType.Data(s));
                StationOperations = stream.ReadArray(s => new EveStationOperation.Data(s));
                NpcCorporations = stream.ReadArray(s => new EveNpcCorporation.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteDateTime(Version);
                stream.WriteArray(Types, x => x.Write(stream));
                stream.WriteArray(StationOperations, x => x.Write(stream));
                stream.WriteArray(NpcCorporations, x => x.Write(stream));
            }
        }

        private readonly Data data;

        public DateTime Version => data.Version;
        public EveTypes Types { get; }
        public EveStationOperations StationOperations { get; }
        public EveNpcCorporations NpcCorporations { get; }

        public EveData(Data data)
        {
            this.data = data;

            Types = new(data.Types.Select(x => new EveType(x)));
            StationOperations = new(data.StationOperations.Select(x => new EveStationOperation(x)));
            NpcCorporations = new(data.NpcCorporations.Select(x => new EveNpcCorporation(x)));
        }

        public EveData(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
