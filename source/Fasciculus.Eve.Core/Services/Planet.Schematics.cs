using Fasciculus.Collections;
using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Services
{
    public interface IPlanetSchematics
    {
        public EvePlanetSchematic this[EveType type] { get; }

        public IReadOnlyList<EvePlanetSchematic> P0 { get; }
        public IReadOnlyList<EvePlanetSchematic> P1 { get; }
        public IReadOnlyList<EvePlanetSchematic> P2 { get; }
        public IReadOnlyList<EvePlanetSchematic> P3 { get; }
        public IReadOnlyList<EvePlanetSchematic> P4 { get; }
    }

    public class PlanetSchematics : IPlanetSchematics
    {
        private readonly EvePlanetSchematic[] p0;
        private readonly EvePlanetSchematic[] p1;
        private readonly EvePlanetSchematic[] p2;
        private readonly EvePlanetSchematic[] p3;
        private readonly EvePlanetSchematic[] p4;

        public IReadOnlyList<EvePlanetSchematic> P0 => p0;
        public IReadOnlyList<EvePlanetSchematic> P1 => p1;
        public IReadOnlyList<EvePlanetSchematic> P2 => p2;
        public IReadOnlyList<EvePlanetSchematic> P3 => p3;
        public IReadOnlyList<EvePlanetSchematic> P4 => p4;

        private readonly Dictionary<EveType, EvePlanetSchematic> byOutputType;

        public EvePlanetSchematic this[EveType type] => byOutputType[type];

        public PlanetSchematics(IEveProvider provider)
        {
            EvePlanetSchematics schematics = provider.PlanetSchematics;
            EveTypes types = provider.Types;

            p0 = FetchP0(schematics.InputTypes, schematics.OutputTypes, types);
            p0.Apply(x => { x.Level = 0; });

            p1 = FetchP1(schematics, p0);
            p1.Apply(x => { x.Level = 1; });

            p2 = FetchP2(schematics, p0, p1);
            p2.Apply(x => { x.Level = 2; });

            p3 = FetchP3(schematics, p0, p1, p2);
            p3.Apply(x => { x.Level = 3; });

            p4 = FetchP4(schematics, p0, p1, p2, p3);
            p4.Apply(x => { x.Level = 4; });

            byOutputType = p0.Concat(schematics).ToDictionary(x => x.OutputType);
        }

        private static EvePlanetSchematic[] FetchP0(IEnumerable<EveType> inputTypes, IEnumerable<EveType> outputTypes, EveTypes types)
        {
            bool filter(EveType x) => !outputTypes.Contains(x);
            EveType[] outputs = [.. inputTypes.Where(filter).OrderBy(x => x.Id)];
            EvePlanetSchematic.Data[] datas = [.. outputs.Select(x => new EvePlanetSchematic.Data(x.Id, x.Name, 1, [], new(x.Id, 1)))];

            return [.. datas.Select(x => new EvePlanetSchematic(x, types))];
        }

        private static EvePlanetSchematic[] FetchP1(EvePlanetSchematics schematics, EvePlanetSchematic[] p0)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            bool filter(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t));

            return [.. schematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP2(EvePlanetSchematics schematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            bool filter1(EvePlanetSchematic x) => x.InputTypes.Any(t => t1.Contains(t));
            bool filter01(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t));
            bool filter(EvePlanetSchematic x) => filter1(x) && filter01(x);

            return [.. schematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP3(EvePlanetSchematics schematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1,
            EvePlanetSchematic[] p2)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            bool filter2(EvePlanetSchematic x) => x.InputTypes.Any(t => t2.Contains(t));
            bool filter012(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t) || t2.Contains(t));
            bool filter(EvePlanetSchematic x) => filter2(x) && filter012(x);

            return [.. schematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP4(EvePlanetSchematics schematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1,
           EvePlanetSchematic[] p2, EvePlanetSchematic[] p3)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            EveType[] t3 = [.. p3.Select(x => x.OutputType)];
            bool filter3(EvePlanetSchematic x) => x.InputTypes.Any(t => t3.Contains(t));
            bool filter0123(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t) || t2.Contains(t) || t3.Contains(t));
            bool filter(EvePlanetSchematic x) => filter3(x) && filter0123(x);

            return [.. schematics.Where(filter).OrderBy(x => x.Id)];
        }
    }
}
