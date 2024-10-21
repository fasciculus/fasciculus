using Fasciculus.IO;

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

        public EveNamedObject(Data data)
            : base(data)
        {
            Name = data.ReadString();
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteString(Name);
        }
    }
}
