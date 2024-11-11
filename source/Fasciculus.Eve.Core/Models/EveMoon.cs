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
            EveId id = BaseRead(stream);

            return new(id);
        }
    }
}
