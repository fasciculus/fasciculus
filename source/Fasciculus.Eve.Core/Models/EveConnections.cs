using Fasciculus.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveConnections
    {
        private readonly SparseBoolMatrix[] solarSystemMatrices;

        private EveConnections(SparseBoolMatrix[] solarSystemMatrices)
        {
            this.solarSystemMatrices = solarSystemMatrices;
        }

        public SparseBoolMatrix GetSolarSystemMatrix(EveSecurityLevel securityLevel)
            => solarSystemMatrices[securityLevel.Index];

        public static EveConnections Create(EveUniverse universe)
        {
            SparseBoolMatrix[] solarSystemMatrices = EveSecurityLevel.Levels.Select(sl => CreateMatrix(universe.SolarSystems, sl)).ToArray();

            return new(solarSystemMatrices);
        }

        private static SparseBoolMatrix CreateMatrix(EveSolarSystems solarSystems, EveSecurityLevel securityLevel)
        {
            SparseBoolVector[] rows = solarSystems.Select(origin => CreateMatrixRow(origin, securityLevel)).ToArray();

            return new(solarSystems.Count, rows);
        }

        private static SparseBoolVector CreateMatrixRow(EveSolarSystem origin, EveSecurityLevel securityLevel)
        {
            IEnumerable<int> indices = origin.Stargates
                .Select(sg => sg.Destination.SolarSystem)
                .Where(securityLevel.Filter)
                .Select(ss => ss.Index);

            return SparseBoolVector.Create(indices);
        }
    }
}
