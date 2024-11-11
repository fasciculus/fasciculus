using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveObject
    {
        public EveId Id { get; }

        public int Index { get; internal set; }

        public EveObject(EveId id)
        {
            Id = id;
        }

        public virtual void Write(Stream stream)
        {
            Id.Write(stream);
        }

        protected static EveId BaseRead(Stream stream)
        {
            return EveId.Read(stream);
        }
    }
}
