using Fasciculus.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveDistances
    {
        private readonly IMatrix<int> distances;

        private EveDistances(IMatrix<int> distances)
        {
            this.distances = distances;
        }

        public static EveDistances Create(IEveUniverse universe, double minSecurity)
        {
            IMatrix<bool> connections = CreateConnections(universe, minSecurity);
            IMutableMatrix<int> distances = InitializeDistances(universe);

            return new(distances);
        }

        private static IMatrix<bool> CreateConnections(IEveUniverse universe, double minSecurity)
        {
            EveSolarSystems solarSystems = universe.SolarSystems;
            IMutableMatrix<bool> connections = Matrices.CreateMutableSparseBool(solarSystems.Count, solarSystems.Count);

            foreach (EveSolarSystem origin in solarSystems)
            {
                IEnumerable<EveSolarSystem> destinations = origin.Stargates
                    .Select(stargate => stargate.Destination.SolarSystem)
                    .Where(destination => destination.Security >= minSecurity);

                destinations.Apply(destination => connections.Set(origin.Index, destination.Index, true));
            }

            return connections.ToMatrix();
        }

        private static IMutableMatrix<int> InitializeDistances(IEveUniverse universe)
        {
            int count = universe.SolarSystems.Count;

            IMutableMatrix<int> distances = Matrices.CreateMutableDenseInt(count, count);

            for (int row = 0; row < count; ++row)
            {
                for (int col = 0; col < count; ++col)
                {
                    distances.Set(row, col, row == col ? 0 : int.MaxValue);
                }
            }

            return distances;
        }
    }
}
