using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class SolarSystem
    {
        public readonly int Id;
        public string Name => Names.Get(Id);

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

            foreach (var entry in sde.stargates)
            {
                Stargate.Create(entry.Key, entry.Value, Id);
            }

            SolarSystems.Add(this);
        }

        public SolarSystem(Data data)
        {
            Id = data.ReadInt();
            constellation = data.ReadInt();
            Security = data.ReadDouble();
            SecurityClass = data.ReadString();

            SolarSystems.Add(this);
        }

        public Stargate[] GetStargates()
            => Stargates.Get(this);

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(constellation);
            data.WriteDouble(Security);
            data.WriteString(SecurityClass);
        }

        public static SolarSystem Create(SdeSolarSystem sde, int constellationId)
            => new(sde, constellationId);

        public static void Read(Data data)
            => new SolarSystem(data);
    }
}
