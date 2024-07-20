using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class SolarSystem
    {
        public readonly int Id;

        private readonly int constellation;
        public Constellation Constellation => Constellations.Get(constellation);

        public SolarSystem(SdeSolarSystem sde, int constellationId)
        {
            Id = sde.solarSystemID;
            constellation = constellationId;
        }

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(constellation);
        }
    }
}
