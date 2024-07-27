using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class Stargate
    {
        public readonly int Id;

        private readonly int destination;

        private readonly int solarSystem;

        public Stargate(int id, SdeStargate sde, int solarSystem)
        {
            Id = id;
            destination = sde.destination;
            this.solarSystem = solarSystem;

            Stargates.Add(this);
        }

        public Stargate(Data data)
        {
            Id = data.ReadInt();
            destination = data.ReadInt();
            solarSystem = data.ReadInt();

            Stargates.Add(this);
        }

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(destination);
            data.WriteInt(solarSystem);
        }

        public static Stargate Create(int id, SdeStargate sde, int solarSystem)
            => new(id, sde, solarSystem);

        public static void Read(Data data)
            => new Stargate(data);
    }
}
