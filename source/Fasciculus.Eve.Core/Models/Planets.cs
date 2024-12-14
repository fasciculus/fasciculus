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
        private readonly Dictionary<EveType, EvePlanetSchematic> byOutput;

        private readonly EveType[] inputTypes;
        private readonly EveType[] outputTypes;

        private readonly EvePlanetSchematic[] p0;
        private readonly EvePlanetSchematic[] p1;
        private readonly EvePlanetSchematic[] p2;
        private readonly EvePlanetSchematic[] p3;
        private readonly EvePlanetSchematic[] p4;

        public IReadOnlyList<EveType> InputTypes => inputTypes;
        public IReadOnlyList<EveType> OutputTypes => outputTypes;

        public IReadOnlyList<EvePlanetSchematic> P0 => p0;
        public IReadOnlyList<EvePlanetSchematic> P1 => p1;
        public IReadOnlyList<EvePlanetSchematic> P2 => p2;
        public IReadOnlyList<EvePlanetSchematic> P3 => p3;
        public IReadOnlyList<EvePlanetSchematic> P4 => p4;

        public EvePlanetSchematic this[EveType type] => byOutput[type];

        public EvePlanetSchematics(IEnumerable<EvePlanetSchematic> planetSchematics, EveTypes types)
        {
            this.planetSchematics = planetSchematics.ToArray();

            inputTypes = FetchInputTypes(this.planetSchematics);
            outputTypes = FetchOutputTypes(this.planetSchematics);

            p0 = FetchP0(inputTypes, outputTypes, types);
            p0.Apply(x => { x.Level = EvePlanetSchematicLevel.P0; });

            p1 = FetchP1(this.planetSchematics, p0);
            p1.Apply(x => { x.Level = EvePlanetSchematicLevel.P1; });

            p2 = FetchP2(this.planetSchematics, p0, p1);
            p2.Apply(x => { x.Level = EvePlanetSchematicLevel.P2; });

            p3 = FetchP3(this.planetSchematics, p0, p1, p2);
            p3.Apply(x => { x.Level = EvePlanetSchematicLevel.P3; });

            p4 = FetchP4(this.planetSchematics, p0, p1, p2, p3);
            p4.Apply(x => { x.Level = EvePlanetSchematicLevel.P4; });

            byOutput = p0.Select(x => Tuple.Create(x.OutputType, x))
                .Concat(this.planetSchematics.Select(x => Tuple.Create(x.OutputType, x)))
                .ToDictionary();
        }

        public IEnumerator<EvePlanetSchematic> GetEnumerator()
            => p0.Concat(planetSchematics).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => p0.Concat(planetSchematics).GetEnumerator();

        private static EveType[] FetchInputTypes(EvePlanetSchematic[] planetSchematics)
            => [.. planetSchematics.SelectMany(x => x.InputTypes).Distinct().OrderBy(x => x.Id)];

        private static EveType[] FetchOutputTypes(EvePlanetSchematic[] planetSchematics)
            => [.. planetSchematics.Select(x => x.OutputType).OrderBy(x => x.Id)];

        private EvePlanetSchematic[] FetchP0(EveType[] inputTypes, EveType[] outputTypes, EveTypes types)
        {
            bool filter(EveType x) => !outputTypes.Contains(x);
            EveType[] outputs = [.. inputTypes.Where(filter).OrderBy(x => x.Id)];
            EvePlanetSchematic.Data[] datas = [.. outputs.Select(x => new EvePlanetSchematic.Data(x.Id, x.Name, 1, [], new(x.Id, 1)))];

            return [.. datas.Select(x => new EvePlanetSchematic(x, types))];
        }

        private static EvePlanetSchematic[] FetchP1(EvePlanetSchematic[] planetSchematics, EvePlanetSchematic[] p0)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            bool filter(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t));

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP2(EvePlanetSchematic[] planetSchematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            bool filter1(EvePlanetSchematic x) => x.InputTypes.Any(t => t1.Contains(t));
            bool filter01(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t));
            bool filter(EvePlanetSchematic x) => filter1(x) && filter01(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP3(EvePlanetSchematic[] planetSchematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1,
            EvePlanetSchematic[] p2)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            bool filter2(EvePlanetSchematic x) => x.InputTypes.Any(t => t2.Contains(t));
            bool filter012(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t) || t2.Contains(t));
            bool filter(EvePlanetSchematic x) => filter2(x) && filter012(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }

        private static EvePlanetSchematic[] FetchP4(EvePlanetSchematic[] planetSchematics, EvePlanetSchematic[] p0, EvePlanetSchematic[] p1,
            EvePlanetSchematic[] p2, EvePlanetSchematic[] p3)
        {
            EveType[] t0 = [.. p0.Select(x => x.OutputType)];
            EveType[] t1 = [.. p1.Select(x => x.OutputType)];
            EveType[] t2 = [.. p2.Select(x => x.OutputType)];
            EveType[] t3 = [.. p3.Select(x => x.OutputType)];
            bool filter3(EvePlanetSchematic x) => x.InputTypes.Any(t => t3.Contains(t));
            bool filter0123(EvePlanetSchematic x) => x.InputTypes.All(t => t0.Contains(t) || t1.Contains(t) || t2.Contains(t) || t3.Contains(t));
            bool filter(EvePlanetSchematic x) => filter3(x) && filter0123(x);

            return [.. planetSchematics.Where(filter).OrderBy(x => x.Id)];
        }
    }

    public class EvePlanetBaseCosts
    {
        private readonly Dictionary<EveType, double> costs;

        public double this[EveType type]
            => costs.TryGetValue(type, out double result) ? result : 0.0;

        public EvePlanetBaseCosts(EvePlanetSchematics schematics)
        {
            costs = schematics.P0.Select(x => Tuple.Create(x.OutputType, 5.0))
                .Concat(schematics.P1.Select(x => Tuple.Create(x.OutputType, 400.0)))
                .Concat(schematics.P2.Select(x => Tuple.Create(x.OutputType, 7_200.0)))
                .Concat(schematics.P3.Select(x => Tuple.Create(x.OutputType, 60_000.0)))
                .Concat(schematics.P4.Select(x => Tuple.Create(x.OutputType, 1_200_000.0)))
                .ToDictionary();
        }
    }

    [DebuggerDisplay("{Type.Name} (x {Quantity})")]
    public class EvePlanetInput
    {
        public EveType Type { get; }
        public EvePlanetSchematicLevel Level { get; }
        public int Quantity { get; }
        public double BuyPrice { get; }
        public double ImportTax { get; }
        public double TotalCost => Quantity * BuyPrice + ImportTax;

        public EvePlanetInput(EveType type, EvePlanetSchematicLevel level, int quantity, double buyPrice, double importTax)
        {
            Type = type;
            Level = level;
            Quantity = quantity;
            BuyPrice = buyPrice;
            ImportTax = importTax;
        }
    }

    public class EvePlanetOutput
    {
        public EveType Type { get; }
        public EvePlanetSchematicLevel Level { get; }
        public int Quantity { get; }
        public double ExportTax { get; }
        public double SellPrice { get; }
        public double GrossIncome => Quantity * SellPrice;
        public double SalesTax { get; }
        public double NetIncome => GrossIncome - ExportTax - SalesTax;

        public EvePlanetOutput(EveType type, EvePlanetSchematicLevel level, int quantity, double exportTax, double sellPrice, double salesTax)
        {
            Type = type;
            Level = level;
            Quantity = quantity;
            ExportTax = exportTax;
            SellPrice = sellPrice;
            SalesTax = salesTax;
        }
    }

    public class EvePlanetProduction
    {
        public EvePlanetOutput Output { get; }

        private readonly EvePlanetInput[] inputs;
        public IReadOnlyList<EvePlanetInput> Inputs => inputs;

        public double Cost { get; }
        public double Income { get; }
        public double Profit => Income - Cost;

        public EvePlanetProduction(EvePlanetSchematic schematic, EvePlanetSchematics schematics, EvePlanetSchematicLevel importLevel,
            EvePlanetBaseCosts baseCosts, EveStationBuyOrders buyOrders, EveStationSellOrders sellOrders, double customsTaxRate,
            double salesTaxRate)
        {
            int count = 3600 / schematic.CycleTime;

            inputs = CreateInputs(schematic, count, schematics, importLevel, baseCosts, sellOrders, customsTaxRate);
            Output = CreateOutput(schematic, baseCosts, buyOrders, customsTaxRate, salesTaxRate);

            Cost = inputs.Sum(x => x.TotalCost);
            Income = Output.NetIncome;
        }

        private static EvePlanetInput[] CreateInputs(EvePlanetSchematic output, int count, EvePlanetSchematics schematics,
            EvePlanetSchematicLevel importLevel, EvePlanetBaseCosts baseCosts, EveStationSellOrders sellOrders,
            double customsTaxRate)
        {
            EvePlanetInput[] inputs = output.Inputs
                .Select(x => Tuple.Create(schematics[x.Type], count * x.Quantity))
                .Select(x => CreateInput(x.Item1, x.Item2, baseCosts, sellOrders, customsTaxRate))
                .ToArray();

            EvePlanetInput[] result = inputs.Where(x => x.Level == importLevel).ToArray();
            EvePlanetInput[] todo = inputs.Where(x => x.Level != importLevel).ToArray();

            while (todo.Length > 0)
            {
                inputs = todo.Select(x => Tuple.Create(schematics[x.Type], x.Quantity))
                    .Select(x => Tuple.Create(x.Item1, x.Item2 / x.Item1.Output.Quantity))
                    .SelectMany(x => CreateInputs(x.Item1, x.Item2, schematics, importLevel, baseCosts, sellOrders, customsTaxRate))
                    .ToArray();

                result = result.Concat(inputs.Where(x => x.Level == importLevel)).ToArray();
                todo = inputs.Where(x => x.Level != importLevel).ToArray();
            }

            return [.. result];
        }

        private static EvePlanetInput CreateInput(EvePlanetSchematic schematic, int quantity, EvePlanetBaseCosts baseCosts,
            EveStationSellOrders sellOrders, double customsTaxRate)
        {
            EveType type = schematic.OutputType;
            EvePlanetSchematicLevel level = schematic.Level;
            double buyPrice = sellOrders[type].PriceFor(quantity * 1000);
            double importTax = quantity * baseCosts[type] * customsTaxRate / 2;

            return new(type, level, quantity, buyPrice, importTax);
        }

        private static EvePlanetOutput CreateOutput(EvePlanetSchematic schematic, EvePlanetBaseCosts baseCosts, EveStationBuyOrders buyOrders,
            double customsTaxRate, double salesTaxRate)
        {
            EveType type = schematic.OutputType;
            EvePlanetSchematicLevel level = schematic.Level;
            int quantity = schematic.Output.Quantity * 3600 / schematic.CycleTime;
            double exportTax = quantity * baseCosts[type] * customsTaxRate;
            double sellPrice = buyOrders[type].PriceFor(quantity * 1000);
            double salesTax = sellPrice * salesTaxRate;

            return new(type, level, quantity, exportTax, sellPrice, salesTax);
        }
    }

    public class EvePlanetProductions : IEnumerable<EvePlanetProduction>
    {
        private readonly EvePlanetProduction[] productions;

        public EvePlanetProductions(EvePlanetSchematics schematics, EveStationBuyOrders buyOrders, EveStationSellOrders sellOrders,
            double customsTaxRate, double salesTaxRate)
        {
            EvePlanetBaseCosts baseCosts = new(schematics);
            EvePlanetSchematicLevel p1 = EvePlanetSchematicLevel.P1;
            EvePlanetSchematicLevel p2 = EvePlanetSchematicLevel.P2;

            var p12 = schematics.P2.Select(x => new EvePlanetProduction(x, schematics, p1, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));
            var p13 = schematics.P3.Select(x => new EvePlanetProduction(x, schematics, p1, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));
            var p23 = schematics.P3.Select(x => new EvePlanetProduction(x, schematics, p2, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));

            productions = [.. p12.Concat(p13).Concat(p23).OrderByDescending(x => x.Profit)];
        }

        public IEnumerator<EvePlanetProduction> GetEnumerator()
            => productions.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => productions.GetEnumerator();
    }
}
