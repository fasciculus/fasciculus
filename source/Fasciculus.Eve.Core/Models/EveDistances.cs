using Fasciculus.Mathematics;
using System;
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

        public int GetMaxDistance()
        {
            int maxDistance = 0;

            for (int row = 0; row < distances.RowCount; ++row)
            {
                for (int col = 0; col < distances.ColumnCount; ++col)
                {
                    maxDistance = Math.Max(maxDistance, distances[row][col]);
                }
            }

            return maxDistance;
        }

        public static EveDistances Create(IEveUniverse universe, double minSecurity)
        {
            SparseBoolMatrix connections = CreateConnections(universe, minSecurity);
            DenseIntMatrix distances = CreateDistances(connections);

            return new(distances);
        }

        private static SparseBoolMatrix CreateConnections(IEveUniverse universe, double minSecurity)
        {
            int count = universe.SolarSystems.Count;

            IEnumerable<MatrixKey> entries = universe.SolarSystems
                .Where(ss => ss.Security >= minSecurity)
                .SelectMany(origin => CreateConnections(origin, minSecurity));

            return SparseBoolMatrix.Create(count, count, entries);
        }

        private static IEnumerable<MatrixKey> CreateConnections(EveSolarSystem origin, double minSecurity)
        {
            return origin.Stargates
                .Select(sg => sg.Destination.SolarSystem)
                .Where(d => d.Security >= minSecurity)
                .Select(ss => MatrixKey.Create(origin.Index, ss.Index));
        }

        private static DenseIntMatrix CreateDistances(SparseBoolMatrix connections)
        {
            int columnCount = connections.ColumnCount;
            DenseIntMatrix matrix = new(columnCount, CreateDistancesRows(connections));

            return matrix + matrix.Transpose();
        }

        private static DenseIntVector[] CreateDistancesRows(SparseBoolMatrix connections)
            => Enumerable.Range(0, connections.RowCount).AsParallel().Select(row => CreateDistancesRow(connections, row)).ToArray();

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
