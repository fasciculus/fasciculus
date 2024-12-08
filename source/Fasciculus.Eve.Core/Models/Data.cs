﻿using System;
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
                Id = stream.ReadInt();
                Name = stream.ReadString();
                ParentId = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteInt(ParentId);
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
    public class EveType
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; } = string.Empty;
            public double Volume { get; }
            public int MarketGroupId { get; }

            public Data(int id, string name, double volume, int marketGroupId)
            {
                Id = id;
                Name = name;
                Volume = volume;
                MarketGroupId = marketGroupId;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Volume = stream.ReadDouble();
                MarketGroupId = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Volume);
                stream.WriteInt(MarketGroupId);
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

    [DebuggerDisplay("{Type.Name} (x{Quantity})")]
    public class EvePlanetSchematicType
    {
        public class Data
        {
            public int Type { get; }
            public int Quantity { get; }

            public Data(int id, int quantity)
            {
                Type = id;
                Quantity = quantity;
            }

            public Data(Stream stream)
            {
                Type = stream.ReadInt();
                Quantity = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Type);
                stream.WriteInt(Quantity);
            }
        }

        private readonly Data data;

        public EveType Type { get; }
        public int Quantity => data.Quantity;

        public EvePlanetSchematicType(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Type];
        }
    }

    [DebuggerDisplay("{Name}")]
    public class EvePlanetSchematic
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public int CycleTime { get; }

            private readonly EvePlanetSchematicType.Data[] inputs;
            public IReadOnlyList<EvePlanetSchematicType.Data> Inputs => inputs;

            public EvePlanetSchematicType.Data Output { get; }

            public Data(int id, string name, int cycleTime, IEnumerable<EvePlanetSchematicType.Data> inputs, EvePlanetSchematicType.Data output)
            {
                Id = id;
                Name = name;
                CycleTime = cycleTime;
                this.inputs = inputs.ToArray();
                Output = output;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                CycleTime = stream.ReadInt();
                inputs = stream.ReadArray(s => new EvePlanetSchematicType.Data(s));
                Output = new EvePlanetSchematicType.Data(stream);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteInt(CycleTime);
                stream.WriteArray(inputs, x => x.Write(stream));
                Output.Write(stream);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;
        public int CycleTime => data.CycleTime;

        private readonly EvePlanetSchematicType[] inputs;
        public IReadOnlyList<EvePlanetSchematicType> Inputs => inputs;

        public EvePlanetSchematicType Output { get; }

        public EvePlanetSchematic(Data data, EveTypes types)
        {
            this.data = data;

            inputs = data.Inputs.Select(x => new EvePlanetSchematicType(x, types)).ToArray();
            Output = new EvePlanetSchematicType(data.Output, types);
        }
    }

    public class EvePlanetSchematics : IEnumerable<EvePlanetSchematic>
    {
        private readonly EvePlanetSchematic[] planetSchematics;

        public EvePlanetSchematics(IEnumerable<EvePlanetSchematic> planetSchematics)
        {
            this.planetSchematics = planetSchematics.ToArray();
        }

        public IEnumerator<EvePlanetSchematic> GetEnumerator()
            => planetSchematics.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => planetSchematics.GetEnumerator();
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

            public Data(DateTime version, IEnumerable<EveMarketGroup.Data> marketGroups, IEnumerable<EveType.Data> types,
                IEnumerable<EveStationOperation.Data> stationOperations, IEnumerable<EveNpcCorporation.Data> npcCorporations,
                IEnumerable<EvePlanetSchematic.Data> planetSchematics)
            {
                Version = version;
                this.marketGroups = marketGroups.ToArray();
                this.types = types.ToArray();
                this.stationOperations = stationOperations.ToArray();
                this.npcCorporations = npcCorporations.ToArray();
                this.planetSchematics = planetSchematics.ToArray();
            }

            public Data(Stream stream)
            {
                Version = stream.ReadDateTime();
                marketGroups = stream.ReadArray(s => new EveMarketGroup.Data(s));
                types = stream.ReadArray(s => new EveType.Data(s));
                stationOperations = stream.ReadArray(s => new EveStationOperation.Data(s));
                npcCorporations = stream.ReadArray(s => new EveNpcCorporation.Data(s));
                planetSchematics = stream.ReadArray(s => new EvePlanetSchematic.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteDateTime(Version);
                stream.WriteArray(marketGroups, x => x.Write(stream));
                stream.WriteArray(types, x => x.Write(stream));
                stream.WriteArray(stationOperations, x => x.Write(stream));
                stream.WriteArray(npcCorporations, x => x.Write(stream));
                stream.WriteArray(planetSchematics, x => x.Write(stream));
            }
        }

        private readonly Data data;

        public DateTime Version => data.Version;
        public EveTypes Types { get; }
        public EveStationOperations StationOperations { get; }
        public EveNpcCorporations NpcCorporations { get; }
        public EvePlanetSchematics PlanetSchematics { get; }

        public EveData(Data data)
        {
            this.data = data;

            Types = new(data.Types.Select(x => new EveType(x)));
            StationOperations = new(data.StationOperations.Select(x => new EveStationOperation(x)));
            NpcCorporations = new(data.NpcCorporations.Select(x => new EveNpcCorporation(x)));
            PlanetSchematics = new(data.PlanetSchematics.Select(x => new EvePlanetSchematic(x, Types)));
        }

        public EveData(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
