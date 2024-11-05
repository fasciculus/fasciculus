using Fasciculus.Mathematics;
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

        public SparseBoolMatrix GetSolarSystemMatrix(EveSecurity security)
            => solarSystemMatrices[security.Index];

        public static EveConnections Create(IEveUniverse universe)
            => new(EveSecurity.Levels.Select(sl => CreateMatrix(universe.SolarSystems, sl)).ToArray());

        private static SparseBoolMatrix CreateMatrix(EveSolarSystems solarSystems, EveSecurity security)
            => new(solarSystems.Count, solarSystems.Select(origin => CreateMatrixRow(origin, security)).ToArray());

        private static SparseBoolVector CreateMatrixRow(EveSolarSystem origin, EveSecurity security)
            => SparseBoolVector.Create(origin.GetNeighbours(security).Select(ss => ss.Index));
    }
}
