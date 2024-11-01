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
            SparseBoolMatrix connections = CreateConnections(universe, minSecurity);
            DenseIntMatrix distances = CreateDistances(connections);

            return new(distances);
        }

        private static SparseBoolMatrix CreateConnections(IEveUniverse universe, double minSecurity)
        {
            EveSolarSystems solarSystems = universe.SolarSystems;

            foreach (EveSolarSystem origin in solarSystems)
            {
                IEnumerable<EveSolarSystem> destinations = origin.Stargates
                    .Select(stargate => stargate.Destination.SolarSystem)
                    .Where(destination => destination.Security >= minSecurity);

                // destinations.Apply(destination => connections.Set(origin.Index, destination.Index, true));
            }

            return SparseBoolMatrix.Create(1, 1, []);
        }

        private static DenseIntMatrix CreateDistances(SparseBoolMatrix connections)
        {
            int columnCount = connections.ColumnCount;
            DenseIntMatrix matrix = new(columnCount, CreateDistancesRows(connections));

            return matrix + matrix.Transpose();
        }

        private static DenseIntVector[] CreateDistancesRows(SparseBoolMatrix connections)
            => Enumerable.Range(0, connections.RowCount).Select(row => CreateDistancesRow(connections, row)).ToArray();

        private static DenseIntVector CreateDistancesRow(SparseBoolMatrix connections, int row)
        {
            int[] result = new int[connections.ColumnCount];
            SparseBoolVector visited = SparseBoolVector.Create(Enumerable.Range(0, row));
            SparseBoolVector front = SparseBoolVector.Create(row);
            int distance = 0;

            while (front.Length())
            {
                ++distance;
                front = connections * front;
                front -= visited;
                front.Indices.Apply(col => result[col] = distance);
                visited += front;
            }

            return new(result);
        }
    }
}
