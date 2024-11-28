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

        protected T? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        protected DataParserBase(IExtractSde extractSde, IYaml yaml)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
        }

        public T Parse()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                ISdeFileSystem sdeFileSystem = extractSde.Extract();

                Report(PendingOrDone.Pending);
                result = Parse(yaml, sdeFileSystem);
                Report(PendingOrDone.Done);
            }

            return result;
        }

        protected abstract T Parse(IYaml yaml, ISdeFileSystem sdeFileSystem);
        protected abstract void Report(PendingOrDone status);
    }

    public interface INamesParser : IDataParserBase<Dictionary<long, string>> { }

    public class NamesParser : DataParserBase<Dictionary<long, string>>, INamesParser
    {
        private readonly IAssetsProgress progress;

        public NamesParser(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
            : base(extractSde, yaml)
        {
            this.progress = progress;
        }

        protected override Dictionary<long, string> Parse(IYaml yaml, ISdeFileSystem sdeFileSystem)
        {
            SdeName[] names = yaml.Deserialize<SdeName[]>(sdeFileSystem.NamesYaml);

            return names.ToDictionary(name => name.ItemID, name => name.ItemName);
        }

        protected override void Report(PendingOrDone status)
            => progress.ReportParseNames(status);
    }

    public interface ITypesParser : IDataParserBase<Dictionary<long, SdeType>> { }

    public class TypesParser : DataParserBase<Dictionary<long, SdeType>>, ITypesParser
    {
        private readonly IAssetsProgress progress;

        public TypesParser(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
            : base(extractSde, yaml)
        {
            this.progress = progress;
        }

        protected override Dictionary<long, SdeType> Parse(IYaml yaml, ISdeFileSystem sdeFileSystem)
        {
            return yaml.Deserialize<Dictionary<long, SdeType>>(sdeFileSystem.TypesYaml);
        }

        protected override void Report(PendingOrDone status)
            => progress.ReportParseTypes(status);
    }

    public interface IDataParser
    {
        public SdeData Parse();
    }

    public class DataParser : IDataParser
    {
        private readonly IDownloadSde downloadSde;
        private readonly INamesParser namesParser;
        private readonly ITypesParser typesParser;

        public DataParser(IDownloadSde downloadSde, INamesParser namesParser, ITypesParser typesParser)
        {
            this.downloadSde = downloadSde;
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
                Version = downloadSde.Download().LastWriteTimeUtc,
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
