using Fasciculus.Eve.Services;
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

        private readonly EveType[] inputTypes;
        public IReadOnlyList<EveType> InputTypes => inputTypes;

        public EvePlanetSchematicType Output { get; }
        public EveType OutputType { get; }

        public int Level { get; internal set; }

        public EvePlanetSchematic(Data data, EveTypes types)
        {
            this.data = data;

            inputs = data.Inputs.Select(x => new EvePlanetSchematicType(x, types)).ToArray();
            inputTypes = [.. inputs.Select(x => x.Type)];

            Output = new EvePlanetSchematicType(data.Output, types);
            OutputType = Output.Type;

            Level = 0;
        }
    }

    public class EvePlanetSchematics : IEnumerable<EvePlanetSchematic>
    {
        private readonly EvePlanetSchematic[] planetSchematics;
        private readonly Dictionary<EveType, EvePlanetSchematic> byOutput;

        private readonly EveType[] inputTypes;
        private readonly EveType[] outputTypes;

        public IReadOnlyList<EveType> InputTypes => inputTypes;
        public IReadOnlyList<EveType> OutputTypes => outputTypes;

        public EvePlanetSchematic this[EveType type] => byOutput[type];

        public EvePlanetSchematics(IEnumerable<EvePlanetSchematic> planetSchematics, EveTypes types)
        {
            this.planetSchematics = planetSchematics.ToArray();

            inputTypes = [.. this.planetSchematics.SelectMany(x => x.InputTypes).Distinct().OrderBy(x => x.Id)];
            outputTypes = [.. this.planetSchematics.Select(x => x.OutputType).OrderBy(x => x.Id)];

            byOutput = this.planetSchematics.ToDictionary(x => x.OutputType);
        }

        public IEnumerator<EvePlanetSchematic> GetEnumerator()
            => planetSchematics.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => planetSchematics.GetEnumerator();
    }

    public class EvePlanetChain
    {

    }

    [DebuggerDisplay("{Type.Name} (x {Quantity})")]
    public class EvePlanetInput
    {
        public EveType Type { get; }
        public int Level { get; }
        public int Quantity { get; }
        public double BuyPrice { get; }
        public double ImportTax { get; }
        public double Cost { get; }

        public EvePlanetInput(EveType type, int level, int quantity, double buyPrice, double importTax)
        {
            Type = type;
            Level = level;
            Quantity = quantity;
            BuyPrice = buyPrice;
            ImportTax = importTax;
            Cost = Quantity * BuyPrice + ImportTax;
        }
    }

    public class EvePlanetOutput
    {
        public EveType Type { get; }
        public int Level { get; }
        public int Quantity { get; }
        public double ExportTax { get; }
        public double SellPrice { get; }
        public double GrossIncome => Quantity * SellPrice;
        public double SalesTax { get; }
        public double NetIncome => GrossIncome - ExportTax - SalesTax;

        public EvePlanetOutput(EveType type, int level, int quantity, double exportTax, double sellPrice, double salesTax)
        {
            Type = type;
            Level = level;
            Quantity = quantity;
            ExportTax = exportTax;
            SellPrice = sellPrice;
            SalesTax = salesTax;
        }
    }

    public class EvePlanetProduction2
    {
        private readonly EvePlanetInput[] inputs;
        public IReadOnlyList<EvePlanetInput> Inputs => inputs;

        public EvePlanetOutput Output { get; }

        public double Cost { get; }
        public double Income { get; }
        public double Profit { get; }

        public EvePlanetProduction2(IEnumerable<EvePlanetInput> inputs, EvePlanetOutput output)
        {
            this.inputs = [.. inputs];

            Output = output;

            Cost = inputs.Sum(x => x.Cost);
            Income = Output.NetIncome;
            Profit = Income - Cost;
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

        public EvePlanetProduction(EvePlanetSchematic schematic, EvePlanetSchematics schematics, int importLevel,
            IPlanetBaseCosts baseCosts, EveStationBuyOrders buyOrders, EveStationSellOrders sellOrders, double customsTaxRate,
            double salesTaxRate)
        {
            int count = 3600 / schematic.CycleTime;

            inputs = CreateInputs(schematic, count, schematics, importLevel, baseCosts, sellOrders, customsTaxRate);
            Output = CreateOutput(schematic, baseCosts, buyOrders, customsTaxRate, salesTaxRate);

            Cost = inputs.Sum(x => x.Cost);
            Income = Output.NetIncome;
        }

        private static EvePlanetInput[] CreateInputs(EvePlanetSchematic output, int count, EvePlanetSchematics schematics,
            int importLevel, IPlanetBaseCosts baseCosts, EveStationSellOrders sellOrders,
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

        private static EvePlanetInput CreateInput(EvePlanetSchematic schematic, int quantity, IPlanetBaseCosts baseCosts,
            EveStationSellOrders sellOrders, double customsTaxRate)
        {
            EveType type = schematic.OutputType;
            int level = schematic.Level;
            double buyPrice = sellOrders[type].PriceFor(quantity * 1000);
            double importTax = quantity * baseCosts[type] * customsTaxRate / 2;

            return new(type, level, quantity, buyPrice, importTax);
        }

        private static EvePlanetOutput CreateOutput(EvePlanetSchematic schematic, IPlanetBaseCosts baseCosts, EveStationBuyOrders buyOrders,
            double customsTaxRate, double salesTaxRate)
        {
            EveType type = schematic.OutputType;
            int level = schematic.Level;
            int quantity = schematic.Output.Quantity * 3600 / schematic.CycleTime;
            double exportTax = quantity * baseCosts[type] * customsTaxRate;
            double sellPrice = buyOrders[type].PriceFor(quantity * 1000);
            double salesTax = sellPrice * salesTaxRate;

            return new(type, level, quantity, exportTax, sellPrice, salesTax);
        }
    }

    //public class EvePlanetProductions : IEnumerable<EvePlanetProduction>
    //{
    //    private readonly EvePlanetProduction[] productions;

    //    public EvePlanetProductions(EvePlanetSchematics schematics, EveStationBuyOrders buyOrders, EveStationSellOrders sellOrders,
    //        double customsTaxRate, double salesTaxRate)
    //    {
    //        EvePlanetBaseCosts baseCosts = new(schematics);

    //        var p12 = schematics.P2.Select(x => new EvePlanetProduction(x, schematics, 1, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));
    //        var p13 = schematics.P3.Select(x => new EvePlanetProduction(x, schematics, 1, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));
    //        var p23 = schematics.P3.Select(x => new EvePlanetProduction(x, schematics, 2, baseCosts, buyOrders, sellOrders, customsTaxRate, salesTaxRate));

    //        productions = [.. p12.Concat(p13).Concat(p23).OrderByDescending(x => x.Profit)];
    //    }

    //    public IEnumerator<EvePlanetProduction> GetEnumerator()
    //        => productions.AsEnumerable().GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator()
    //        => productions.GetEnumerator();
    //}
}
