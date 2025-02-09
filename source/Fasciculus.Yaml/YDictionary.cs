using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Fasciculus.Yaml
{
    public class YDictionary
    {
        private Dictionary<string, string> entries;

        private YDictionary(Dictionary<string, string> entries)
        {
            this.entries = entries;
        }

        public string GetString(string key, string? defaultValue = null)
            => entries.TryGetValue(key, out string? value) ? value : defaultValue ?? string.Empty;

        public static YDictionary Deserialize(string data, IDeserializer deserializer)
        {
            Dictionary<string, string> entries = deserializer.Deserialize<Dictionary<string, string>>(data);

            return new(entries);
        }

        public static YDictionary Deserialize(string data)
            => Deserialize(data, YDeserializer.Default);

        public static YDictionary Empty { get; } = new([]);
    }
}
