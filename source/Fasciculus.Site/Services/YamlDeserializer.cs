using YamlDotNet.Serialization;

namespace Fasciculus.Site.Services
{
    public class YamlDeserializer
    {
        private readonly IDeserializer deserializer;

        public YamlDeserializer()
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
