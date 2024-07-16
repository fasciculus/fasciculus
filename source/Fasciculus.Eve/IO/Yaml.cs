using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Fasciculus.Eve.IO
{
    public static class Yaml
    {
        public static async Task<T> DeserializeAsync<T>(string input)
        {
            await Task.Delay(0);

            return new DeserializerBuilder().IgnoreUnmatchedProperties().Build().Deserialize<T>(input);
        }

        public static async Task<T> DeserializeAsync<T>(FileInfo file)
            => await DeserializeAsync<T>(file.ReadAllText());
    }
}
