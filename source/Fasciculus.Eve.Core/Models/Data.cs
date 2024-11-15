using System.IO;

namespace Fasciculus.Eve.Models
{
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
            stream.WriteArray(objectsByIndex, c => c.Write(stream));
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
            stream.WriteArray(objectsByIndex, c => c.Write(stream));
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

        public EveType(EveId id, EveId groupId, EveId marketGroupId)
            : base(id)
        {
            this.groupId = groupId;
            this.marketGroupId = marketGroupId;
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

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, t => t.Write(stream));
        }

        public static EveTypes Read(Stream stream)
        {
            EveType[] types = stream.ReadArray(EveType.Read);

            return new(types);
        }
    }

    public class EveData
    {
        public required EveNames Names { get; init; }
        public required EveNpcCorporations NpcCorporations { get; init; }
        public required EveStationOperations StationOperations { get; init; }
        public required EveTypes Types { get; init; }

        public static EveData Read(Stream stream)
        {
            EveNames names = EveNames.Read(stream);
            EveNpcCorporations npcCorporations = EveNpcCorporations.Read(stream);
            EveStationOperations stationOperations = EveStationOperations.Read(stream);
            EveTypes types = EveTypes.Read(stream);

            return new()
            {
                Names = names,
                NpcCorporations = npcCorporations,
                StationOperations = stationOperations,
                Types = types
            };
        }

        public void Write(Stream stream)
        {
            Names.Write(stream);
            NpcCorporations.Write(stream);
            StationOperations.Write(stream);
            Types.Write(stream);
        }
    }
}
