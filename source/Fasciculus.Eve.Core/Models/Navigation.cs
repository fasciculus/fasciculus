using Fasciculus.Mathematics;
using Fasciculus.Validating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveDistances
    {
        private readonly EveSolarSystems solarSystems;
        private readonly DenseShortMatrix distances;

        public EveDistances(EveSolarSystems solarSystems, DenseShortMatrix distances)
        {
            this.solarSystems = solarSystems;
            this.distances = distances;
        }

        public short GetDistance(EveSolarSystem origin, EveSolarSystem destination)
        {
            int row = origin.Index;
            int column = destination.Index;

            if (row == column)
            {
                return 0;
            }

            short distance = distances[row][column];

            return distance > 0 ? distance : short.MaxValue;
        }

        public IEnumerable<EveSolarSystem> AtRange(EveSolarSystem origin, int distance)
        {
            if (distance < 1)
            {
                return [origin];
            }

            DenseShortVector row = distances[origin.Index];

            return Enumerable.Range(0, distances.ColumnCount).Where(i => row[i] == distance).Select(i => solarSystems[i]);
        }

        public short GetMaxDistance()
        {
            short maxDistance = 0;

            for (int row = 0; row < distances.RowCount; ++row)
            {
                for (int col = 0; col < distances.ColumnCount; ++col)
                {
                    maxDistance = Math.Max(maxDistance, distances[row][col]);
                }
            }

            return maxDistance;
        }

        public void Write(Stream stream)
        {
            int n = solarSystems.Count;

            for (int r = 0; r < n; ++r)
            {
                DenseShortVector row = distances[r];

                for (int c = r + 1; c < n; ++c)
                {
                    stream.WriteShort(row[c]);
                }
            }
        }

        public static EveDistances Read(EveSolarSystems solarSystems, Stream stream)
        {
            int n = solarSystems.Count;
            List<DenseShortVector> rows = new();

            for (int r = 0; r < n; ++r)
            {
                short[] values = new short[n];

                for (int c = r + 1; c < n; ++c)
                {
                    values[c] = stream.ReadShort();
                }

                rows.Add(new DenseShortVector(values));
            }

            DenseShortMatrix distances = new(n, rows.ToArray());

            distances = distances + distances.Transpose();

            return new(solarSystems, distances);
        }
    }

    public class EveNavigation
    {
        private readonly EveDistances[] distances;

        public EveNavigation(EveDistances[] distances)
        {
            this.distances = distances;
        }

        public short GetMaxDistance(EveSecurity security)
        {
            return distances[security.Index].GetMaxDistance();
        }

        public short GetDistance(EveSolarSystem origin, EveSolarSystem destination, EveSecurity security)
        {
            return distances[security.Index].GetDistance(origin, destination);
        }

        public IEnumerable<EveSolarSystem> AtRange(EveSolarSystem origin, int distance, EveSecurity security)
        {
            return distances[security.Index].AtRange(origin, distance);
        }

        public EveSolarSystem Nearest(EveSolarSystem origin, EveSecurity security, Func<EveSolarSystem, bool> predicate)
        {
            EveSolarSystem? nearest = null;

            for (short distance = 0; distance < 1000 && nearest is null; ++distance)
            {
                nearest = AtRange(origin, distance, security).FirstOrDefault(predicate);
            }

            return Cond.NotNull(nearest);
        }

        public void Write(Stream stream)
        {
            stream.WriteArray(distances, distance => distance.Write(stream));
        }

        public static EveNavigation Read(IEveUniverse universe, Stream stream)
        {
            EveDistances[] distances = stream.ReadArray(_ => EveDistances.Read(universe.SolarSystems, stream));

            return new(distances);
        }
    }
}
