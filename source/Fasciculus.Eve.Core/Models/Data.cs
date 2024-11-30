using System;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveData
    {
        public class Data
        {
            public DateTime Version { get; }

            public Data(DateTime version)
            {
                Version = version;
            }

            public Data(Stream stream)
            {
                Version = stream.ReadDateTime();
            }

            public void Write(Stream stream)
            {
                stream.WriteDateTime(Version);
            }
        }

        private readonly Data data;

        public EveData(Data data)
        {
            this.data = data;
        }

        public EveData(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
