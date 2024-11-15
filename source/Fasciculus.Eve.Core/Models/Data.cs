using Fasciculus.Validating;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveMarketGroup : EveObject
    {
        private readonly EveId parentId;

        public required string Name { get; init; }

        public EveMarketGroup(EveId id, EveId parentId)
            : base(id)
        {
            this.parentId = parentId;
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            parentId.Write(stream);
            stream.WriteString(Name);
        }

        public static EveMarketGroup Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            EveId parentId = EveId.Read(stream);
            string name = stream.ReadString();

            return new(id, parentId)
            {
                Name = name
            };
        }
    }

    public class EveMarketGroups : EveObjects<EveMarketGroup>
    {
        public EveMarketGroups(EveMarketGroup[] marketGroups)
            : base(marketGroups) { }

        public void Write(Stream stream)
        {
            stream.WriteArray(objects, mg => mg.Write(stream));
        }

        public static EveMarketGroups Read(Stream stream)
        {
            EveMarketGroup[] marketGroups = stream.ReadArray(EveMarketGroup.Read);

            return new(marketGroups);
        }
    }

    public class EveNpcCorporation : EveObject
    {
        public string Name { get; }

        public EveNpcCorporation(EveId id, string name)
            : base(id)
        {
            Name = name;
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            stream.WriteString(Name);
        }

        public static EveNpcCorporation Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            string name = stream.ReadString();

            return new(id, name);
        }
    }

    public class EveNpcCorporations : EveObjects<EveNpcCorporation>
    {
        public EveNpcCorporations(EveNpcCorporation[] npcCorporations)
            : base(npcCorporations) { }

        public void Write(Stream stream)
        {
            stream.WriteArray(objects, c => c.Write(stream));
        }

        public static EveNpcCorporations Read(Stream stream)
        {
            EveNpcCorporation[] npcCorporations = stream.ReadArray(EveNpcCorporation.Read);

            return new(npcCorporations);
        }
    }

    public class EveStationOperation : EveObject
    {
        public string Name { get; }

        public EveStationOperation(EveId id, string name)
            : base(id)
        {
            Name = name;
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            stream.WriteString(Name);
        }

        public static EveStationOperation Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            string name = stream.ReadString();

            return new(id, name);
        }
    }

    public class EveStationOperations : EveObjects<EveStationOperation>
    {
        public EveStationOperations(EveStationOperation[] stationOperations)
            : base(stationOperations) { }

        public void Write(Stream stream)
        {
            stream.WriteArray(objects, c => c.Write(stream));
        }

        public static EveStationOperations Read(Stream stream)
        {
            EveStationOperation[] stationOperations = stream.ReadArray(EveStationOperation.Read);

            return new(stationOperations);
        }
    }

    public class EveType : EveObject
    {
        private EveId groupId;
        private EveId marketGroupId;

        public required string Name { get; init; }
        public required int PortionSize { get; init; }
        public required bool Published { get; init; }
        public required double Volume { get; init; }

        private EveMarketGroup? marketGroup;

        public EveMarketGroup MarketGroup
            => Cond.NotNull(marketGroup);

        public bool IsTradeable
            => Published && marketGroup != null;

        public EveType(EveId id, EveId groupId, EveId marketGroupId)
            : base(id)
        {
            this.groupId = groupId;
            this.marketGroupId = marketGroupId;
        }

        internal void Link(EveMarketGroups marketGroups)
        {
            if (marketGroups.TryGet(marketGroupId, out EveMarketGroup? marketGroup))
            {
                this.marketGroup = marketGroup;
            }
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            groupId.Write(stream);
            marketGroupId.Write(stream);
            stream.WriteString(Name);
            stream.WriteInt(PortionSize);
            stream.WriteBool(Published);
            stream.WriteDouble(Volume);
        }

        public static EveType Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            EveId groupId = EveId.Read(stream);
            EveId marketGroupId = EveId.Read(stream);
            string name = stream.ReadString();
            int portionSize = stream.ReadInt();
            bool published = stream.ReadBool();
            double volume = stream.ReadDouble();

            return new EveType(id, groupId, marketGroupId)
            {
                Name = name,
                PortionSize = portionSize,
                Published = published,
                Volume = volume
            };
        }
    }

    public class EveTypes : EveObjects<EveType>
    {
        public EveTypes(EveType[] types)
            : base(types) { }

        internal void Link(EveMarketGroups marketGroups)
        {
            objects.Apply(mg => mg.Link(marketGroups));
        }

        public void Write(Stream stream)
        {
            stream.WriteArray(objects, t => t.Write(stream));
        }

        public static EveTypes Read(Stream stream)
        {
            EveType[] types = stream.ReadArray(EveType.Read);

            return new(types);
        }
    }

    public class EveData
    {
        public EveMarketGroups MarketGroups { get; }
        public EveNames Names { get; }
        public EveNpcCorporations NpcCorporations { get; }
        public EveStationOperations StationOperations { get; }
        public EveTypes Types { get; }

        public EveData(EveMarketGroups marketGroups, EveNames names, EveNpcCorporations npcCorporations, EveStationOperations stationOperations,
            EveTypes types)
        {
            MarketGroups = marketGroups;
            Names = names;
            NpcCorporations = npcCorporations;
            StationOperations = stationOperations;
            Types = types;

            Types.Link(MarketGroups);
        }

        public static EveData Read(Stream stream)
        {
            EveMarketGroups marketGroups = EveMarketGroups.Read(stream);
            EveNames names = EveNames.Read(stream);
            EveNpcCorporations npcCorporations = EveNpcCorporations.Read(stream);
            EveStationOperations stationOperations = EveStationOperations.Read(stream);
            EveTypes types = EveTypes.Read(stream);

            return new(marketGroups, names, npcCorporations, stationOperations, types);
        }

        public void Write(Stream stream)
        {
            MarketGroups.Write(stream);
            Names.Write(stream);
            NpcCorporations.Write(stream);
            StationOperations.Write(stream);
            Types.Write(stream);
        }
    }
}
