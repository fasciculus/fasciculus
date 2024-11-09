using Fasciculus.IO;
using System.Collections.Generic;

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

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteArray(moons, moon => moon.Write(data));
        }

        public static EvePlanet Read(Data data)
        {
            EveId id = EveId.Read(data);
            EveMoon[] moons = data.ReadArray(EveMoon.Read);

            return new(id, moons);
        }
    }
}
