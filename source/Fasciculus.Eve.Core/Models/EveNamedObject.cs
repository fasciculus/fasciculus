using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveNamedObject : EveObject
    {
        public string Name { get; }

        public EveNamedObject(EveId id, string name)
            : base(id)
        {
            Name = name;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteString(Name);
        }

        protected static new (EveId id, string name) BaseRead(Stream stream)
        {
            EveId id = EveObject.BaseRead(stream);
            string name = stream.ReadString();

            return (id, name);
        }
    }
}
