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
                Type = stream.ReadInt();
                Quantity = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Type);
                stream.WriteInt(Quantity);
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
