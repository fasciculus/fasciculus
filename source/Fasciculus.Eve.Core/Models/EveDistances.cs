using Fasciculus.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveDistances
    {
        private readonly EveSolarSystems solarSystems;
        private readonly SparseShortMatrix distances;

        public EveDistances(EveSolarSystems solarSystems, SparseShortMatrix distances)
        {
            this.solarSystems = solarSystems;
            this.distances = distances;
        }

        public int GetDistance(EveSolarSystem origin, EveSolarSystem destination)
        {
            int row = origin.Index;
            int column = destination.Index;

            if (row == column)
            {
                return 0;
            }

            int distance = distances[row][column];

            return distance > 0 ? distance : int.MaxValue;
        }

        public IEnumerable<EveSolarSystem> AtRange(EveSolarSystem origin, int distance)
        {
            if (distance < 1)
            {
                return [];
            }

            SparseShortVector row = distances[origin.Index];

            return Enumerable.Range(0, distances.ColumnCount).Where(i => row[i] == distance).Select(i => solarSystems[i]);
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
    }

    public static class EveDistancesFactory
    {
        public static EveDistances Create(IEveUniverse universe, EveConnections connections, EveSecurity security)
        {
            EveSolarSystems solarSystems = universe.SolarSystems;
            SparseBoolMatrix connectionMatrix = connections.GetSolarSystemMatrix(security);
            DenseShortMatrix distances = CreateDistances(connectionMatrix);

            return new(solarSystems, distances.ToSparse());
        }

        private static DenseShortMatrix CreateDistances(SparseBoolMatrix connections)
        {
            int columnCount = connections.ColumnCount;

            return new(columnCount, CreateDistancesRows(connections));
        }

        private static DenseShortVector[] CreateDistancesRows(SparseBoolMatrix connections)
            => Enumerable.Range(0, connections.RowCount).AsParallel().Select(row => CreateDistancesRow(connections, row)).ToArray();

        private static DenseShortVector CreateDistancesRow(SparseBoolMatrix connections, int row)
        {
            short[] result = new short[connections.ColumnCount];
            SparseBoolVector visited = SparseBoolVector.Create();
            SparseBoolVector front = SparseBoolVector.Create(row);
            short distance = 0;

            while (front.Length())
            {
                ++distance;
                visited += front;
                front = connections * front;
                front -= visited;
                ApplyDistances(front, distance, result);
            }

            return new(result);
        }

        private static void ApplyDistances(SparseBoolVector front, short distance, short[] result)
        {
            foreach (int index in front.Indices)
            {
                int existing = result[index];

                if (existing == 0 || existing > distance)
                {
                    result[index] = distance;
                }
            }
        }
    }
}
