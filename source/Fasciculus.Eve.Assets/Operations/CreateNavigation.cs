using Fasciculus.Eve.Models;
using Fasciculus.Mathematics;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Fasciculus.Eve.Operations
{
    public static class CreateNavigation
    {
        private class SubProgress : IProgress<int>
        {
            private readonly IProgress<string> progress;
            private readonly long step;

            private long done = 0;

            public SubProgress(IEveUniverse universe, IProgress<string> progress)
            {
                this.progress = progress;
                step = (universe.SolarSystems.Count * 3) / 100;
            }

            public void Report(int value)
            {
                long current = Interlocked.Increment(ref done);

                if (current % step == 0)
                {
                    progress.Report($"  distances {current / step} %");
                }
            }
        }

        public static EveNavigation Execute(IEveUniverse universe, IProgress<string> progress)
        {
            progress.Report("Creating Navigation");

            Stopwatch stopwatch = Stopwatch.StartNew();
            SubProgress subProgress = new(universe, progress);
            EveDistances[] solarSystemDistances = CreateSolarSystemDistances(universe.SolarSystems, subProgress);

            progress.Report($"Creating Navigation done {stopwatch.Elapsed}");

            return new(solarSystemDistances);
        }

        private static EveDistances[] CreateSolarSystemDistances(EveSolarSystems solarSystems, IProgress<int> progress)
        {
            EveConnections connections = CreateConnections(solarSystems);

            return EveSecurity.Levels.Select(security => CreateSolarSystemDistances(solarSystems, connections, security, progress)).ToArray();
        }

        private static EveConnections CreateConnections(EveSolarSystems solarSystems)
        {
            return new(EveSecurity.Levels.Select(security => CreateConnections(solarSystems, security)).ToArray());
        }

        private static SparseBoolMatrix CreateConnections(EveSolarSystems solarSystems, EveSecurity security)
        {
            return new(solarSystems.Count, solarSystems.Select(origin => CreateConnectionsRow(origin, security)).ToArray());
        }

        private static SparseBoolVector CreateConnectionsRow(EveSolarSystem origin, EveSecurity security)
        {
            return SparseBoolVector.Create(origin.GetNeighbours(security).Select(ss => ss.Index));
        }

        private static EveDistances CreateSolarSystemDistances(EveSolarSystems solarSystems, EveConnections connections, EveSecurity security, IProgress<int> progress)
        {
            SparseBoolMatrix connectionMatrix = connections.GetSolarSystemMatrix(security);
            DenseShortMatrix distances = CreateSolarSystemDistances(connectionMatrix, progress);

            return new(solarSystems, distances);
        }

        private static DenseShortMatrix CreateSolarSystemDistances(SparseBoolMatrix connections, IProgress<int> progress)
        {
            int columnCount = connections.ColumnCount;

            return new(columnCount, CreateSolarSystemDistancesRows(connections, progress));
        }

        private static DenseShortVector[] CreateSolarSystemDistancesRows(SparseBoolMatrix connections, IProgress<int> progress)
        {
            return Enumerable.Range(0, connections.RowCount).AsParallel().Select(row => CreateSolarSystemDistancesRow(connections, row, progress)).ToArray();
        }

        private static DenseShortVector CreateSolarSystemDistancesRow(SparseBoolMatrix connections, int row, IProgress<int> progress)
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

            progress.Report(1);

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
