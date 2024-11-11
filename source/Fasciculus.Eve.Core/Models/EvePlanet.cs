using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EvePlanet : EveObject
    {
        private readonly EveMoon[] moons;

        public IEnumerable<EveMoon> Moons
            => moons;

        public EvePlanet(EveId id, EveMoon[] moons)
            : base(id)
        {
            this.moons = moons;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            Data data = stream;

            data.WriteArray(moons, moon => moon.Write(data));
        }

        public static EvePlanet Read(Stream stream)
        {
            Data data = stream;

            EveId id = EveId.Read(data);
            EveMoon[] moons = data.ReadArray(EveMoon.Read);

            return new(id, moons);
        }
    }
}
