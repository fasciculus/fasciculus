using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Fasciculus.Yaml
{
    public class YDocument
    {
        private readonly Dictionary<string, string> entries;
        private readonly IDeserializer deserializer;

        private YDocument(Dictionary<string, string> entries, IDeserializer deserializer)
        {
            this.entries = entries;
            this.deserializer = deserializer;
        }

        public string GetString(string key, string? defaultValue = null)
            => entries.TryGetValue(key, out string? value) ? value : defaultValue ?? string.Empty;

        public static YDocument Deserialize(string data, IDeserializer deserializer)
        {
            Dictionary<string, string> entries = deserializer.Deserialize<Dictionary<string, string>>(data);

            return new(entries, deserializer);
        }

        public static YDocument Deserialize(string data)
            => Deserialize(data, YDeserializer.Instance);

        public static YDocument Empty(IDeserializer deserializer)
            => new([], deserializer);

        public static YDocument Empty()
            => Empty(YDeserializer.Instance);
    }
}
