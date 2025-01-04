using Fasciculus.IO;
using YamlDotNet.Serialization;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IYaml
    {
        public T Deserialize<T>(FileInfo file);
    }

    public class Yaml : IYaml
    {
        private readonly IDeserializer deserializer;

        public Yaml()
        {
            deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().WithCaseInsensitivePropertyMatching().Build();
        }

        public T Deserialize<T>(FileInfo file)
        {
            return deserializer.Deserialize<T>(file.ReadAllText());
        }
    }
}
