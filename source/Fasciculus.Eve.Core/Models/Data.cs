using Fasciculus.Collections;
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
    public class EveMarketGroup
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;
            public int ParentId { get; }

            public Data(int id, string name, int parentId)
            {
                Id = id;
                Name = name;
                ParentId = parentId;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt32();
                Name = stream.ReadString();
                ParentId = stream.ReadInt32();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Id);
                stream.WriteString(Name);
                stream.WriteInt32(ParentId);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;
        public EveMarketGroup? Parent { get; private set; }

        public EveMarketGroup(Data data)
        {
            this.data = data;
        }

        internal void SetParent(IReadOnlyDictionary<int, EveMarketGroup> marketGroups)
        {
            if (marketGroups.TryGetValue(data.ParentId, out EveMarketGroup? parent))
            {
                Parent = parent;
            }
        }
    }

    public class EveMarketGroups : IEnumerable<EveMarketGroup>
    {
        private readonly EveMarketGroup[] marketGroups;
        private readonly Dictionary<int, EveMarketGroup> byId;

        public int Count => marketGroups.Length;

        public bool Contains(int id) => byId.ContainsKey(id);
        public EveMarketGroup? this[int id] => byId.TryGetValue(id, out EveMarketGroup? result) ? result : null;

        public EveMarketGroups(IEnumerable<EveMarketGroup> marketGroups)
        {
            this.marketGroups = marketGroups.ToArray();

            byId = this.marketGroups.ToDictionary(x => x.Id);
            this.marketGroups.Apply(x => { x.SetParent(byId); });
        }

        public IEnumerator<EveMarketGroup> GetEnumerator()
            => marketGroups.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => marketGroups.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
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
                Id = stream.ReadInt32();
                Name = stream.ReadString();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Id);
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
        private readonly EveStationOperation[] stationOperations;
        private readonly Lazy<Dictionary<int, EveStationOperation>> byId;

        public EveStationOperation this[int id] => byId.Value[id];

        public EveStationOperations(IEnumerable<EveStationOperation> stationOperations)
        {
            this.stationOperations = stationOperations.ToArray();
            byId = new(() => this.stationOperations.ToDictionary(x => x.Id), true);
        }

        public IEnumerator<EveStationOperation> GetEnumerator()
            => stationOperations.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => stationOperations.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
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
                Id = stream.ReadInt32();
                Name = stream.ReadString();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Id);
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
        private readonly EveNpcCorporation[] npcCorporations;
        private readonly Lazy<Dictionary<int, EveNpcCorporation>> byId;

        public EveNpcCorporation this[int id] => byId.Value[id];

        public EveNpcCorporations(IEnumerable<EveNpcCorporation> npcCorporations)
        {
            this.npcCorporations = npcCorporations.ToArray();
            byId = new(() => this.npcCorporations.ToDictionary(x => x.Id), true);
        }

        public IEnumerator<EveNpcCorporation> GetEnumerator()
            => npcCorporations.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => npcCorporations.GetEnumerator();
    }

    public class EveData
    {
        public class Data
        {
            public DateTime Version { get; }

            private readonly EveMarketGroup.Data[] marketGroups;
            public IReadOnlyList<EveMarketGroup.Data> MarketGroups => marketGroups;

            private readonly EveType.Data[] types;
            public IReadOnlyList<EveType.Data> Types => types;

            private readonly EveStationOperation.Data[] stationOperations;
            public IReadOnlyList<EveStationOperation.Data> StationOperations => stationOperations;

            private readonly EveNpcCorporation.Data[] npcCorporations;
            public IReadOnlyList<EveNpcCorporation.Data> NpcCorporations => npcCorporations;

            private readonly EvePlanetSchematic.Data[] planetSchematics;
            public IReadOnlyList<EvePlanetSchematic.Data> PlanetSchematics => planetSchematics;

            private readonly EveBlueprint.Data[] blueprints;
            public IReadOnlyList<EveBlueprint.Data> Blueprints => blueprints;

            public Data(DateTime version, IEnumerable<EveMarketGroup.Data> marketGroups, IEnumerable<EveType.Data> types,
                IEnumerable<EveStationOperation.Data> stationOperations, IEnumerable<EveNpcCorporation.Data> npcCorporations,
                IEnumerable<EvePlanetSchematic.Data> planetSchematics, IEnumerable<EveBlueprint.Data> blueprints)
            {
                Version = version;
                this.marketGroups = marketGroups.ToArray();
                this.types = types.ToArray();
                this.stationOperations = stationOperations.ToArray();
                this.npcCorporations = npcCorporations.ToArray();
                this.planetSchematics = planetSchematics.ToArray();
                this.blueprints = blueprints.ToArray();
            }

            public Data(Stream stream)
            {
                Version = stream.ReadDateTime();
                marketGroups = stream.ReadArray(s => new EveMarketGroup.Data(s));
                types = stream.ReadArray(s => new EveType.Data(s));
                stationOperations = stream.ReadArray(s => new EveStationOperation.Data(s));
                npcCorporations = stream.ReadArray(s => new EveNpcCorporation.Data(s));
                planetSchematics = stream.ReadArray(s => new EvePlanetSchematic.Data(s));
                blueprints = stream.ReadArray(s => new EveBlueprint.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteDateTime(Version);
                stream.WriteArray(marketGroups, (s, x) => x.Write(s));
                stream.WriteArray(types, (s, x) => x.Write(s));
                stream.WriteArray(stationOperations, (s, x) => x.Write(s));
                stream.WriteArray(npcCorporations, (s, x) => x.Write(s));
                stream.WriteArray(planetSchematics, (s, x) => x.Write(s));
                stream.WriteArray(blueprints, (s, x) => x.Write(s));
            }
        }

        private readonly Data data;

        public DateTime Version => data.Version;
        public EveMarketGroups MarketGroups { get; }
        public EveTypes Types { get; }
        public EveStationOperations StationOperations { get; }
        public EveNpcCorporations NpcCorporations { get; }
        public EvePlanetSchematics PlanetSchematics { get; }
        public EveBlueprints Blueprints { get; }

        public EveData(Data data)
        {
            this.data = data;

            MarketGroups = new(data.MarketGroups.Select(x => new EveMarketGroup(x)));
            Types = new(data.Types.Select(x => new EveType(x, MarketGroups)));
            StationOperations = new(data.StationOperations.Select(x => new EveStationOperation(x)));
            NpcCorporations = new(data.NpcCorporations.Select(x => new EveNpcCorporation(x)));
            PlanetSchematics = new(data.PlanetSchematics.Select(x => new EvePlanetSchematic(x, Types)), Types);
            Blueprints = new(data.Blueprints.Select(x => new EveBlueprint(x, Types)));
        }

        public EveData(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
