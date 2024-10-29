using Fasciculus.Collections;
using Fasciculus.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveDistances
    {
        private readonly DenseIntMatrix distances;

        private EveDistances(DenseIntMatrix distances)
        {
            this.distances = distances;
        }

        public static EveDistances Create(IEveUniverse universe, double minSecurity)
        {
            SparseBoolMatrix connections = CollectConnections(universe, minSecurity);
            DenseIntMatrix distances = CalculateDistances(connections);

            return new(distances);
        }

        private static SparseBoolMatrix CollectConnections(IEveUniverse universe, double minSecurity)
        {
            EveSolarSystems solarSystems = universe.SolarSystems;
            MutableSparseBoolMatrix connections = MutableSparseBoolMatrix.Create(solarSystems.Count, solarSystems.Count);

            foreach (EveSolarSystem origin in solarSystems)
            {
                IEnumerable<EveSolarSystem> destinations = origin.Stargates
                    .Select(stargate => stargate.Destination.SolarSystem)
                    .Where(destination => destination.Security >= minSecurity);

                destinations.Apply(destination => connections.Set(origin.Index, destination.Index, true));
            }

            return connections.ToMatrix();
        }

        private static MutableDenseIntMatrix InitializeDistances(int count)
        {
            MutableDenseIntMatrix distances = MutableDenseIntMatrix.Create(count, count);

            for (int row = 0; row < count; ++row)
            {
                for (int col = 0; col < count; ++col)
                {
                    distances.Set(row, col, row == col ? 0 : int.MaxValue);
                }
            }

            return distances;
        }

        private static DenseIntMatrix CalculateDistances(SparseBoolMatrix connections)
        {
            int rowCount = connections.RowCount;
            MutableDenseIntMatrix distances = InitializeDistances(rowCount);

            Enumerable.Range(0, rowCount).AsParallel().Select(row => CalculateDistances(connections, row, distances)).ToArray();

            return distances.ToMatrix();
        }

        private static bool CalculateDistances(SparseBoolMatrix connections, int row, MutableDenseIntMatrix distances)
        {
            BitSet visited = BitSet.Create();
            BitSet front = BitSet.Create(row);
            int distance = 0;

            while (front.Count > 0)
            {
                ++distance;
                front = connections * front;
                front -= visited;

                front.Apply(col => distances.Set(row, col, distance));

                visited += front;
            }

            return true;
        }
    }
}
