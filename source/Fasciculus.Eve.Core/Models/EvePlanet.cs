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

            stream.WriteArray(moons, moon => moon.Write(stream));
        }

        public static EvePlanet Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveMoon[] moons = stream.ReadArray(EveMoon.Read);

            return new(id, moons);
        }
    }
}
