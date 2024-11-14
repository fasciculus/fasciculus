using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveName : EveNamedObject
    {
        public EveName(EveId id, string name)
            : base(id, name) { }
    }

    public class EveNames : EveObjects<EveName>
    {
        public EveNames(EveName[] names)
            : base(names) { }

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, n => n.Write(stream));
        }
    }

    public class EveData
    {
        public EveNames Names { get; }

        public EveData(EveNames names)
        {
            Names = names;
        }

        public void Write(Stream stream)
        {
            Names.Write(stream);
        }
    }
}
