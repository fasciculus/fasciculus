using Fasciculus.IO;
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

            Data data = stream;

            destinationId.Write(data);
        }

        public static EveStargate Read(Stream stream)
        {
            Data data = stream;

            EveId id = EveId.Read(data);
            EveId destinationId = EveId.Read(data);

            return new EveStargate(id, destinationId);
        }
    }
}
