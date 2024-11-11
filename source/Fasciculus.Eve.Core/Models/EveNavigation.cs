using Fasciculus.IO;
using Fasciculus.Validating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public short GetDistance(EveSolarSystem origin, EveSolarSystem destination, EveSecurity security)
        {
            return distances[security.Index].GetDistance(origin, destination);
        }

        public IEnumerable<EveSolarSystem> AtRange(EveSolarSystem origin, int distance, EveSecurity security)
        {
            return distances[security.Index].AtRange(origin, distance);
        }

        public EveSolarSystem Nearest(EveSolarSystem origin, EveSecurity security, Func<EveSolarSystem, bool> predicate)
        {
            EveSolarSystem? nearest = null;

            for (short distance = 0; distance < 1000 && nearest is null; ++distance)
            {
                nearest = AtRange(origin, distance, security).FirstOrDefault(predicate);
            }

            return Cond.NotNull(nearest);
        }

        public void Write(Stream stream)
        {
            Data data = stream;

            data.WriteArray(distances, d => d.Write(data));
        }

        public static EveNavigation Read(IEveUniverse universe, Stream stream)
        {
            Data data = stream;

            EveDistances[] distances = data.ReadArray(d => EveDistances.Read(universe.SolarSystems, d));

            return new(distances);
        }
    }
}
