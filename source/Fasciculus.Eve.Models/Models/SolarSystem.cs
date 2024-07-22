using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class SolarSystem
    {
        public readonly int Id;

        private readonly int constellation;
        public Constellation Constellation => Constellations.Get(constellation);

        public readonly double Security;
        public bool Safe => Security >= 0.5;

        public readonly string SecurityClass;

        public SolarSystem(SdeSolarSystem sde, int constellationId)
        {
            Id = sde.solarSystemID;
            constellation = constellationId;
            Security = sde.security;
            SecurityClass = sde.securityClass;
        }

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(constellation);
            data.WriteDouble(Security);
            data.WriteString(SecurityClass);
        }
    }
}
