using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO;
using YamlDotNet.Serialization;

namespace Fasciculus.Eve.Services
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

    public static class YamlServices
    {
        public static IServiceCollection AddYaml(this IServiceCollection services)
        {
            services.TryAddSingleton<IYaml, Yaml>();

            return services;
        }
    }
}
