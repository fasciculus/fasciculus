using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
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
    }

    public class EveManufacturing
    {
        public class Data
        {
            public int Time { get; }

            private EveMaterial.Data[] materials;
            public IReadOnlyList<EveMaterial.Data> Materials => materials;

            private EveMaterial.Data[] products;
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
    }

    public class EveBlueprint
    {
        public class Data
        {
            public int Id { get; }
            public EveManufacturing.Data Manufacturing { get; }

            public Data(int id, EveManufacturing.Data manufacturing)
            {
                Id = id;
                Manufacturing = manufacturing;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Manufacturing = new EveManufacturing.Data(stream);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                Manufacturing.Write(stream);
            }
        }
    }

    public class EveBlueprints
    {
    }
}
