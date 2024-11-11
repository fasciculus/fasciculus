using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveMoon : EveObject
    {
        public EveMoon(EveId id)
            : base(id) { }

        public override void Write(Stream stream)
        {
            base.Write(stream);
        }

        public static EveMoon Read(Stream stream)
        {
            Data data = stream;

            EveId id = EveId.Read(data);

            return new(id);
        }
    }
}
