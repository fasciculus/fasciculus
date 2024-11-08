using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        private readonly EveDistances[] distances;

        public EveNavigation(EveDistances[] distances)
        {
            this.distances = distances;
        }

        public short GetMaxDistance(EveSecurity security)
        {
            return distances[security.Index].GetMaxDistance();
        }

        public void Write(Data data)
        {
            data.WriteArray(distances, d => d.Write(data));
        }

        public static EveNavigation Read(IEveUniverse universe, Data data)
        {
            EveDistances[] distances = data.ReadArray(d => EveDistances.Read(universe.SolarSystems, d));

            return new(distances);
        }
    }
}
