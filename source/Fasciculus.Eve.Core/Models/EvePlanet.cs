using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EvePlanet : EveObject
    {
        public EvePlanet(EveId id)
            : base(id)
        {
        }

        public override void Write(Data data)
        {
            base.Write(data);
        }

        public static EvePlanet Read(Data data)
        {
            EveId id = EveId.Read(data);

            return new(id);
        }
    }
}
