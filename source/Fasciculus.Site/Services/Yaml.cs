using YamlDotNet.Serialization;

namespace Fasciculus.Site.Services
{
    public class Yaml
    {
        private readonly IDeserializer deserializer;

        public Yaml()
        {
            deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithCaseInsensitivePropertyMatching()
                .Build();
        }

        public T Deserialize<T>(string text)
        {
            return deserializer.Deserialize<T>(text);
        }
    }
}
