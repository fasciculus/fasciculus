using Fasciculus.IO.Binary;
using System.Diagnostics;
using System.IO;

namespace Fasciculus.Eve.Models
{
    [DebuggerDisplay("{Type.Name} (x{Quantity})")]
    public class EvePlanetTypeQuantity
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
                Type = stream.ReadInt32();
                Quantity = stream.ReadInt32();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt32(Type);
                stream.WriteInt32(Quantity);
            }
        }

        public EveType Type { get; }
        public int Quantity { get; }

        public EvePlanetTypeQuantity(Data data, EveTypes types)
            : this(types[data.Type], data.Quantity) { }

        internal EvePlanetTypeQuantity(EveType type, int quantity)
        {
            Type = type;
            Quantity = quantity;
        }
    }
}
