using System.IO;
using YamlDotNet.Serialization;

namespace Fasciculus.Eve
{
    public static class Yaml
    {
        public static IDeserializer CreateDeserializer()
            => new DeserializerBuilder().IgnoreUnmatchedProperties().WithCaseInsensitivePropertyMatching().Build();

        public static T Deserialize<T>(FileInfo file)
            => CreateDeserializer().Deserialize<T>(file.ReadAllText());
    }
}
