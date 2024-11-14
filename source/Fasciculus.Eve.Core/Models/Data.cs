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

    public class EveData
    {
        public required EveNames Names { get; init; }
        public required EveNpcCorporations NpcCorporations { get; init; }

        public static EveData Read(Stream stream)
        {
            EveNames names = EveNames.Read(stream);
            EveNpcCorporations npcCorporations = EveNpcCorporations.Read(stream);

            return new()
            {
                Names = names,
                NpcCorporations = npcCorporations
            };
        }

        public void Write(Stream stream)
        {
            Names.Write(stream);
            NpcCorporations.Write(stream);
        }
    }
}
