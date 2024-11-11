using Fasciculus.Validating;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveStargate : EveObject
    {
        private readonly EveId destinationId;

        private EveSolarSystem? solarSystem;
        private EveStargate? destination;

        public EveSolarSystem SolarSystem => Cond.NotNull(solarSystem);
        public EveStargate Destination => Cond.NotNull(destination);

        public EveStargate(EveId id, EveId destinationId)
            : base(id)
        {
            this.destinationId = destinationId;
        }

        public void Link(EveSolarSystem solarSystem, IEveUniverse universe)
        {
            this.solarSystem = solarSystem;
            destination = universe.Stargates[destinationId];
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            destinationId.Write(stream);
        }

        public static EveStargate Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveId destinationId = EveId.Read(stream);

            return new EveStargate(id, destinationId);
        }
    }
}
