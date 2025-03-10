using Fasciculus.IO.Binary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    [DebuggerDisplay("{Name}")]
    public class EveType
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;
            public double Volume { get; }
            public int MetaGroupId { get; }
            public int MarketGroupId { get; }

            public Data(int id, string name, double volume, int metaGroupId, int marketGroupId)
            {
                Id = id;
                Name = name;
                Volume = volume;
                MetaGroupId = metaGroupId;
                MarketGroupId = marketGroupId;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt32();
                Name = stream.ReadString();
                Volume = stream.ReadDouble();
                MetaGroupId = stream.ReadInt32();
                MarketGroupId = stream.ReadInt32();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Volume);
                stream.WriteInt32(MetaGroupId);
                stream.WriteInt32(MarketGroupId);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;
        public double Volume => data.Volume;

        public int MetaGroup => data.MetaGroupId;
        public EveMarketGroup? MarketGroup { get; }

        public EveType(Data data, EveMarketGroups marketGroups)
        {
            this.data = data;

            MarketGroup = marketGroups[data.MarketGroupId];
        }
    }

    public class EveTypes : IEnumerable<EveType>
    {
        private readonly EveType[] types;
        private readonly Lazy<Dictionary<int, EveType>> byId;

        public int Count => types.Length;

        public bool Contains(int id) => byId.Value.ContainsKey(id);
        public EveType this[int id] => byId.Value[id];

        public EveTypes(IEnumerable<EveType> types)
        {
            this.types = types.ToArray();
            byId = new(() => types.ToDictionary(x => x.Id), true);
        }

        public IEnumerator<EveType> GetEnumerator()
            => types.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => types.GetEnumerator();
    }
}
