using Fasciculus.IO;
using System.Diagnostics;

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

            public Data(BinaryRW bin)
            {
                Type = bin.ReadInt();
                Quantity = bin.ReadInt();
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteInt(Type);
                bin.WriteInt(Quantity);
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
