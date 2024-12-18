using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Services
{
    public interface IPlanetBaseCosts
    {
        public double this[EveType type] { get; }
    }

    public class PlanetBaseCosts : IPlanetBaseCosts
    {
        private readonly Dictionary<EveType, double> costs;

        public double this[EveType type]
            => costs.TryGetValue(type, out double result) ? result : 0.0;

        public PlanetBaseCosts(IPlanetSchematics schematics)
        {
            costs = schematics.P0.Select(x => Tuple.Create(x.OutputType, 5.0))
                .Concat(schematics.P1.Select(x => Tuple.Create(x.OutputType, 400.0)))
                .Concat(schematics.P2.Select(x => Tuple.Create(x.OutputType, 7_200.0)))
                .Concat(schematics.P3.Select(x => Tuple.Create(x.OutputType, 60_000.0)))
                .Concat(schematics.P4.Select(x => Tuple.Create(x.OutputType, 1_200_000.0)))
                .ToDictionary();
        }
    }
}
