using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveData
    {
        public EveNames Names { get; }

        public EveData(EveNames names)
        {
            Names = names;
        }

        public static EveData Read(Stream stream)
        {
            EveNames names = EveNames.Read(stream);

            return new(names);
        }

        public void Write(Stream stream)
        {
            Names.Write(stream);
        }
    }
}
