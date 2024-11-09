using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveMoon : EveObject
    {
        public EveMoon(EveId id)
            : base(id) { }

        public override void Write(Data data)
        {
            base.Write(data);
        }

        public static EveMoon Read(Data data)
        {
            EveId id = EveId.Read(data);

            return new(id);
        }
    }
}
