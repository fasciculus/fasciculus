using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveObject
    {
        public EveId Id { get; }

        public EveObject(EveId id)
        {
            Id = id;
        }

        public EveObject(Data data)
        {
            Id = EveId.Read(data);
        }

        public virtual void Write(Data data)
        {
            Id.Write(data);
        }
    }
}
