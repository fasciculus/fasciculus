﻿using System.IO;
using YamlDotNet.Serialization;

namespace Fasciculus.Eve.Utilities
{
    public static class Yaml
    {
        public static IDeserializer CreateDeserializer()
            => new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

        public static T Deserialize<T>(FileInfo file)
            => CreateDeserializer().Deserialize<T>(file.ReadAllText());
    }
}
