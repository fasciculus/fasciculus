using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveData
    {
        public EveData() { }

        private EveData(Data data)
        {

        }

        public void Write(Data data)
        {
        }

        public static EveData Read(Data data)
            => new(data);
    }
}
