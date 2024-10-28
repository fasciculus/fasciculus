using Fasciculus.Collections;
using Fasciculus.Mathematics;
using System;
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
            IMatrix<bool> connections = CollectConnections(universe, minSecurity);
            IMatrix<int> distances = CalculateDistances(connections);

            return new(distances);
        }

        private static IMatrix<bool> CollectConnections(IEveUniverse universe, double minSecurity)
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

        private static IMatrix<int> CalculateDistances(IMatrix<bool> connections)
        {
            int rowCount = connections.RowCount;
            MutableDenseIntMatrix distances = InitializeDistances(rowCount);

            for (int row = 0; row < rowCount; ++row)
            {
                BitSet visited = BitSet.Create();
                BitSet front = BitSet.Create(row);
                int distance = 0;

                while (front.Count > 0)
                {
                    ++distance;
                    front = BitSet.Create(connections.Mul(SparseBoolVector.Create(front)).Select(e => e.Index));
                    front -= visited;

                    front.Apply(col => distances.Set(row, col, Math.Min(distances.Get(row, col), distance)));

                    visited += front;
                }
            }

            return distances.ToMatrix();
        }
    }
}
