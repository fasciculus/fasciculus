using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Fasciculus.Yaml
{
    public class YDeserializer : IDeserializer
    {
        private readonly IDeserializer deserializer;

        public static YDeserializer Default { get; } = new();

        private YDeserializer()
        {
            deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithCaseInsensitivePropertyMatching()
                .Build();
        }

        public T Deserialize<T>(string input)
            => deserializer.Deserialize<T>(input);

        public T Deserialize<T>(TextReader input)
            => deserializer.Deserialize<T>(input);

        public T Deserialize<T>(IParser parser)
            => deserializer.Deserialize<T>(parser);

        public object? Deserialize(string input)
            => deserializer.Deserialize(input);

        public object? Deserialize(TextReader input)
            => deserializer.Deserialize(input);

        public object? Deserialize(IParser parser)
            => deserializer.Deserialize(parser);

        public object? Deserialize(string input, Type type)
            => deserializer.Deserialize(input, type);

        public object? Deserialize(TextReader input, Type type)
            => deserializer.Deserialize(input, type);

        public object? Deserialize(IParser parser, Type type)
            => deserializer.Deserialize(parser, type);
    }
}
