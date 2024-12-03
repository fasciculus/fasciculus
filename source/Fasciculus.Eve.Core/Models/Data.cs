using System;
using System.IO;

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

        public EveType(Data data)
        {
            this.data = data;
        }
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

        public EveData(Data data)
        {
            this.data = data;
        }

        public EveData(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
