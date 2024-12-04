using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public interface IEveFileSystem
    {
        public DirectoryInfo Documents { get; }
        public FileInfo TradeOptions { get; }
    }

    public class EveFileSystem : IEveFileSystem
    {
        public DirectoryInfo Documents { get; }
        public FileInfo TradeOptions => Documents.File("TradeOptions.json");

        public EveFileSystem(ISpecialDirectories specialDirectories)
        {

            Documents = specialDirectories.Documents.Combine("Fasciculus", "Eve").CreateIfNotExists();
        }
    }

    public static class EveFileSystemServices
    {
        public static IServiceCollection AddEveFileSystem(this IServiceCollection services)
        {
            services.AddSpecialDirectories();

            services.TryAddSingleton<IEveFileSystem, EveFileSystem>();

            return services;
        }
    }
}
