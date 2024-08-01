using Fasciculus.Mathematics;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class Adjacencies
    {
        public static Adjacencies Create(IEnumerable<SolarSystem> solarSystems)
        {
            SolarSystemMapping mapping = new(solarSystems);
            SparseMatrixFactoryInt factory = new(mapping.Count);

            foreach (SolarSystem origin in solarSystems)
            {
                int originIndex = mapping[origin];

                foreach (Stargate stargate in origin.GetStargates())
                {
                    SolarSystem destination = stargate.Destination.SolarSystem;

                    if (mapping.Contains(destination))
                    {
                        int destinationIndex = mapping[destination];

                        factory.Set(originIndex, destinationIndex, 1);
                    }
                }
            }

            return new();
        }
    }
}
