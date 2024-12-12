using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    [DebuggerDisplay("{Type.Name} (x{Quantity})")]
    public class EvePlanetSchematicType
    {
        public class Data
        {
            public int Type { get; }
            public int Quantity { get; }

            public Data(int id, int quantity)
            {
                Type = id;
                Quantity = quantity;
            }

            public Data(Stream stream)
            {
                Type = stream.ReadInt();
                Quantity = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Type);
                stream.WriteInt(Quantity);
            }
        }

        private readonly Data data;

        public EveType Type { get; }
        public int Quantity => data.Quantity;

        public EvePlanetSchematicType(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Type];
        }
    }

    public enum EvePlanetSchematicLevel
    {
        P0,
        P1,
        P2,
        P3,
        P4
    }

    [DebuggerDisplay("{Name}")]
    public class EvePlanetSchematic
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public int CycleTime { get; }

            private readonly EvePlanetSchematicType.Data[] inputs;
            public IReadOnlyList<EvePlanetSchematicType.Data> Inputs => inputs;

            public EvePlanetSchematicType.Data Output { get; }

            public Data(int id, string name, int cycleTime, IEnumerable<EvePlanetSchematicType.Data> inputs, EvePlanetSchematicType.Data output)
            {
                Id = id;
                Name = name;
                CycleTime = cycleTime;
                this.inputs = inputs.ToArray();
                Output = output;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                CycleTime = stream.ReadInt();
                inputs = stream.ReadArray(s => new EvePlanetSchematicType.Data(s));
                Output = new EvePlanetSchematicType.Data(stream);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteInt(CycleTime);
                stream.WriteArray(inputs, x => x.Write(stream));
                Output.Write(stream);
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;
        public int CycleTime => data.CycleTime;

        private readonly EvePlanetSchematicType[] inputs;
        public IReadOnlyList<EvePlanetSchematicType> Inputs => inputs;
        public IEnumerable<EveType> InputTypes => Inputs.Select(x => x.Type);

        public EvePlanetSchematicType Output { get; }
        public EveType OutputType => Output.Type;

        public EvePlanetSchematicLevel Level { get; internal set; } = EvePlanetSchematicLevel.P0;

        public EvePlanetSchematic(Data data, EveTypes types)
        {
            this.data = data;

            inputs = data.Inputs.Select(x => new EvePlanetSchematicType(x, types)).ToArray();
            Output = new EvePlanetSchematicType(data.Output, types);
        }
    }

    public class EvePlanetSchematics : IEnumerable<EvePlanetSchematic>
    {
        private readonly EvePlanetSchematic[] planetSchematics;

        private readonly EveType[] inputTypes;
        private readonly EveType[] outputTypes;

        private readonly EveType[] p0;
        private readonly EvePlanetSchematic[] p1;
        private readonly EvePlanetSchematic[] p2;
        private readonly EvePlanetSchematic[] p3;
        private readonly EvePlanetSchematic[] p4;

        public IReadOnlyList<EveType> InputTypes => inputTypes;
        public IReadOnlyList<EveType> OutputTypes => outputTypes;

        public IReadOnlyList<EveType> P0 => p0;
        public IReadOnlyList<EvePlanetSchematic> P1 => p1;
        public IReadOnlyList<EvePlanetSchematic> P2 => p2;
        public IReadOnlyList<EvePlanetSchematic> P3 => p3;
        public IReadOnlyList<EvePlanetSchematic> P4 => p4;

        public EvePlanetSchematics(IEnumerable<EvePlanetSchematic> planetSchematics)
        {
            this.planetSchematics = planetSchematics.ToArray();

            inputTypes = FetchInputTypes(this.planetSchematics);
            outputTypes = FetchOutputTypes(this.planetSchematics);

            p0 = FetchP0(inputTypes, outputTypes);

            p1 = FetchP1(this.planetSchematics, p0);
            p1.Apply(x => { x.Level = EvePlanetSchematicLevel.P1; });

            p2 = FetchP2(this.planetSchematics, p0, p1);
            p2.Apply(x => { x.Level = EvePlanetSchematicLevel.P2; });

            p3 = FetchP3(this.planetSchematics, p0, p1, p2);
            p3.Apply(x => { x.Level = EvePlanetSchematicLevel.P3; });

            p4 = FetchP4(this.planetSchematics, p0, p1, p2, p3);
            p4.Apply(x => { x.Level = EvePlanetSchematicLevel.P4; });
        }

        public IEnumerator<EvePlanetSchematic> GetEnumerator()
            => planetSchematics.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => planetSchematics.GetEnumerator();

        private static EveType[] FetchInputTypes(EvePlanetSchematic[] planetSchematics)
            => [.. planetSchematics.SelectMany(x => x.InputTypes).Distinct().OrderBy(x => x.Id)];

        private static EveType[] FetchOutputTypes(EvePlanetSchematic[] planetSchematics)
            => [.. planetSchematics.Select(x => x.OutputType).OrderBy(x => x.Id)];

        private static EveType[] FetchP0(EveType[] inputTypes, EveType[] outputTypes)
        {
            bool filter(EveType x) => !outputTypes.Contains(x);

            return [.. inputTypes.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP1(EvePlanetSchematic[] planetSchematics, EveType[] p0)
        {
            bool filter(EvePlanetSchematic x) => x.InputTypes.All(t => p0.Contains(t));

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP2(EvePlanetSchematic[] planetSchematics, EveType[] p0, EvePlanetSchematic[] p1)
        {
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            bool filter1(EvePlanetSchematic x) => x.InputTypes.Any(t => t1.Contains(t));
            bool filter01(EvePlanetSchematic x) => x.InputTypes.All(t => p0.Contains(t) || t1.Contains(t));
            bool filter(EvePlanetSchematic x) => filter1(x) && filter01(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP3(EvePlanetSchematic[] planetSchematics, EveType[] p0, EvePlanetSchematic[] p1,
            EvePlanetSchematic[] p2)
        {
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            bool filter2(EvePlanetSchematic x) => x.InputTypes.Any(t => t2.Contains(t));
            bool filter012(EvePlanetSchematic x) => x.InputTypes.All(t => p0.Contains(t) || t1.Contains(t) || t2.Contains(t));
            bool filter(EvePlanetSchematic x) => filter2(x) && filter012(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP4(EvePlanetSchematic[] planetSchematics, EveType[] p0, EvePlanetSchematic[] p1,
            EvePlanetSchematic[] p2, EvePlanetSchematic[] p3)
        {
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            EveType[] t3 = [.. p3.Select(x => x.OutputType)];
            bool filter3(EvePlanetSchematic x) => x.InputTypes.Any(t => t3.Contains(t));
            bool filter0123(EvePlanetSchematic x) => x.InputTypes.All(t => p0.Contains(t) || t1.Contains(t) || t2.Contains(t) || t3.Contains(t));
            bool filter(EvePlanetSchematic x) => filter3(x) && filter0123(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }
    }
}
