using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IDataParserBase<T>
        where T : class
    {
        public T Parse();
    }

    public abstract class DataParserBase<T> : IDataParserBase<T>
        where T : class
    {
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IProgress<PendingOrDone> progress;

        protected T? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        protected DataParserBase(IExtractSde extractSde, IYaml yaml, IProgress<PendingOrDone> progress)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        public T Parse()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                ISdeFileSystem sdeFileSystem = extractSde.Extract();

                progress.Report(PendingOrDone.Pending);
                result = Parse(yaml, sdeFileSystem);
                progress.Report(PendingOrDone.Done);
            }

            return result;
        }

        protected abstract T Parse(IYaml yaml, ISdeFileSystem sdeFileSystem);
    }

    public interface INamesParser : IDataParserBase<Dictionary<long, string>> { }

    public class NamesParser : DataParserBase<Dictionary<long, string>>, INamesParser
    {
        public NamesParser(IExtractSde extractSde, IYaml yaml,
            [FromKeyedServices(ServiceKeys.NamesParser)] IProgress<PendingOrDone> progress)
            : base(extractSde, yaml, progress) { }

        protected override Dictionary<long, string> Parse(IYaml yaml, ISdeFileSystem sdeFileSystem)
        {
            SdeName[] names = yaml.Deserialize<SdeName[]>(sdeFileSystem.NamesYaml);

            return names.ToDictionary(name => name.ItemID, name => name.ItemName);
        }
    }

    public interface ITypesParser : IDataParserBase<Dictionary<long, SdeType>> { }

    public class TypesParser : DataParserBase<Dictionary<long, SdeType>>, ITypesParser
    {
        public TypesParser(IExtractSde extractSde, IYaml yaml,
            [FromKeyedServices(ServiceKeys.TypesParser)] IProgress<PendingOrDone> progress)
            : base(extractSde, yaml, progress) { }

        protected override Dictionary<long, SdeType> Parse(IYaml yaml, ISdeFileSystem sdeFileSystem)
        {
            return yaml.Deserialize<Dictionary<long, SdeType>>(sdeFileSystem.TypesYaml);
        }
    }

    public interface IDataParser
    {
        public SdeData Parse();
    }

    public class DataParser : IDataParser
    {
        private readonly INamesParser namesParser;
        private readonly ITypesParser typesParser;

        public DataParser(INamesParser namesParser, ITypesParser typesParser)
        {
            this.namesParser = namesParser;
            this.typesParser = typesParser;
        }

        public SdeData Parse()
        {
            Task<Dictionary<long, string>> names = Tasks.LongRunning(ParseNames);
            Task<Dictionary<long, SdeType>> types = Tasks.LongRunning(ParseTypes);

            Task[] tasks = { names, types };

            Task.WaitAll(tasks);

            return new()
            {
                Names = names.Result,
                Types = types.Result,
            };
        }

        private Dictionary<long, string> ParseNames()
            => namesParser.Parse();

        private Dictionary<long, SdeType> ParseTypes()
            => typesParser.Parse();
    }

    public static class DataParserServices
    {
        public static IServiceCollection AddDataParsers(this IServiceCollection services)
        {
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<INamesParser, NamesParser>();
            services.TryAddSingleton<ITypesParser, TypesParser>();
            services.TryAddSingleton<IDataParser, DataParser>();

            return services;
        }
    }
}
