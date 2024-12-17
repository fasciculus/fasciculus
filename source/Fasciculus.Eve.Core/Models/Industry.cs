using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveIndustryIndices
    {
        public class Data
        {
            private readonly Dictionary<int, double> indices;
            public IReadOnlyDictionary<int, double> Indices => indices;

            public Data(IReadOnlyDictionary<int, double> indices)
            {
                this.indices = new(indices);
            }

            public Data(Stream stream)
            {
                indices = stream.ReadDictionary(s => s.ReadInt(), s => s.ReadDouble());
            }

            public void Write(Stream stream)
            {
                stream.WriteDictionary(indices, stream.WriteInt, stream.WriteDouble);
            }
        }

        private readonly Data data;

        public double this[EveSolarSystem solarSystem]
            => data.Indices.TryGetValue(solarSystem.Id, out double value) ? value : 0;

        public EveIndustryIndices(Data data)
        {
            this.data = data;
        }
    }

    public class EveMaterial
    {
        public class Data
        {
            public int Type { get; }
            public int Quantity { get; }

            public Data(int type, int quantity)
            {
                Type = type;
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

        public EveMaterial(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Type];
        }
    }

    public class EveManufacturing : ISkillConsumer
    {
        public class Data
        {
            public int Time { get; }

            private EveMaterial.Data[] materials;
            public IReadOnlyList<EveMaterial.Data> Materials => materials;

            private readonly EveMaterial.Data[] products;
            public IReadOnlyList<EveMaterial.Data> Products => products;

            private EveSkill.Data[] skills;
            public IReadOnlyList<EveSkill.Data> Skills => skills;

            public Data(int time, IEnumerable<EveMaterial.Data> materials, IEnumerable<EveMaterial.Data> products,
                IEnumerable<EveSkill.Data> skills)
            {
                Time = time;

                this.materials = materials.ToArray();
                this.products = products.ToArray();
                this.skills = skills.ToArray();
            }

            public Data(Stream stream)
            {
                Time = stream.ReadInt();

                materials = stream.ReadArray(s => new EveMaterial.Data(s));
                products = stream.ReadArray(s => new EveMaterial.Data(s));
                skills = stream.ReadArray(s => new EveSkill.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Time);

                stream.WriteArray(materials, x => x.Write(stream));
                stream.WriteArray(products, x => x.Write(stream));
                stream.WriteArray(skills, x => x.Write(stream));
            }
        }

        private readonly Data data;
        private readonly EveMaterial[] materials;
        private readonly EveMaterial[] products;
        private readonly EveSkill[] skills;

        public int Time => data.Time;
        public IReadOnlyList<EveMaterial> Materials => materials;
        public IReadOnlyList<EveMaterial> Products => products;
        public IEnumerable<ISkill> RequiredSkills => skills;

        public EveManufacturing(Data data, EveTypes types)
        {
            this.data = data;

            materials = [.. data.Materials.Select(x => new EveMaterial(x, types))];
            products = [.. data.Products.Select(x => new EveMaterial(x, types))];
            skills = [.. data.Skills.Select(x => new EveSkill(x, types))];
        }

        public bool RequiresSkill(EveType skillType)
            => skills.Any(x => x.Type == skillType);
    }

    [DebuggerDisplay("{Type.Name}")]
    public class EveBlueprint
    {
        public class Data
        {
            public int Id { get; }
            public int MaxRuns { get; }

            public EveManufacturing.Data Manufacturing { get; }

            public Data(int id, int maxRuns, EveManufacturing.Data manufacturing)
            {
                Id = id;
                MaxRuns = maxRuns;
                Manufacturing = manufacturing;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                MaxRuns = stream.ReadInt();
                Manufacturing = new EveManufacturing.Data(stream);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(MaxRuns);
                Manufacturing.Write(stream);
            }
        }

        private readonly Data data;

        public EveType Type { get; }
        public int MaxRuns => data.MaxRuns;
        public EveManufacturing Manufacturing { get; }

        public EveBlueprint(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Id];
            Manufacturing = new(data.Manufacturing, types);
        }
    }

    public class EveBlueprints : IEnumerable<EveBlueprint>
    {
        private readonly EveBlueprint[] blueprints;

        public EveBlueprints(IEnumerable<EveBlueprint> blueprints)
        {
            this.blueprints = blueprints.ToArray();
        }

        public IEnumerator<EveBlueprint> GetEnumerator()
            => blueprints.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => blueprints.GetEnumerator();
    }

    public class EveProductionInput
    {
        public EveType Type { get; }
        public int Quantity { get; }
        public double Cost { get; }
        public double Volume { get; }

        public EveProductionInput(EveType type, int quantity, double cost)
        {
            Type = type;
            Quantity = quantity;
            Cost = cost;
            Volume = Math.Ceiling(type.Volume * Quantity);
        }
    }

    public class EveProductionOutput
    {
        public EveType Type { get; }
        public int Quantity { get; }
        public double Income { get; }
        public double Volume { get; }

        public EveProductionOutput(EveType type, int quantity, double income)
        {
            Type = type;
            Quantity = quantity;
            Income = income;
            Volume = Math.Ceiling(Type.Volume * Quantity);
        }
    }

    [DebuggerDisplay("{Name}")]
    public class EveProduction
    {
        private const int SecondsPerDay = 24 * 60 * 60;

        public EveBlueprint Blueprint { get; }
        public double BlueprintPrice { get; }

        private readonly EveProductionInput[] inputs;
        public IReadOnlyList<EveProductionInput> Inputs => inputs;

        private readonly EveProductionOutput[] outputs;
        public IReadOnlyList<EveProductionOutput> Outputs => outputs;

        public string Name => outputs.Length > 0 ? outputs[0].Type.Name : Blueprint.Type.Name;

        public int Runs { get; }
        public double RunsPerDay { get; }
        public double MaterialCost { get; }
        public double JobCost { get; }
        public double SalesTax { get; }
        public double Cost { get; }
        public double Income { get; }
        public double Profit { get; }
        public double Margin { get; }
        public double OutputVolume { get; }

        public EveProduction(EveBlueprint blueprint, double blueprintPrice, int runs,
            IEnumerable<EveProductionInput> inputs, IEnumerable<EveProductionOutput> outputs,
            double jobCost, double salesTaxRate)
        {
            double untaxedIncome = outputs.Sum(x => x.Income);

            Blueprint = blueprint;
            BlueprintPrice = blueprintPrice;

            Runs = runs;
            RunsPerDay = (1.0 * SecondsPerDay) / Math.Max(1, blueprint.Manufacturing.Time);

            this.inputs = inputs.ToArray();
            this.outputs = outputs.ToArray();

            MaterialCost = inputs.Sum(x => x.Cost);
            JobCost = jobCost;
            SalesTax = untaxedIncome * salesTaxRate;
            Cost = MaterialCost + JobCost + SalesTax;
            Income = untaxedIncome;
            Profit = Income - Cost;
            Margin = Profit / Math.Max(1, Cost) * 100;
            OutputVolume = outputs.Sum(x => x.Volume);
        }
    }
}
