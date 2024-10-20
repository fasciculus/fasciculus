using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveRegion
    {
        public int Id { get; }
        public string Name { get; }

        public int Index { get; internal set; }

        public EveRegion(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteString(Name);
        }
    }
}
