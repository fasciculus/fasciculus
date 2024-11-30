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

    public interface IParseNames : IDataParserBase<Dictionary<long, string>> { }

    public class ParseNames : DataParserBase<Dictionary<long, string>>, IParseNames
    {
        private readonly IAssetsProgress progress;

        public ParseNames(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
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
            => progress.ParseNames.Report(status);
    }

    public interface IParseTypes : IDataParserBase<Dictionary<long, SdeType>> { }

    public class ParseTypes : DataParserBase<Dictionary<long, SdeType>>, IParseTypes
    {
        private readonly IAssetsProgress progress;

        public ParseTypes(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
            : base(extractSde, yaml)
        {
            this.progress = progress;
        }

        protected override Dictionary<long, SdeType> Parse(IYaml yaml, ISdeFileSystem sdeFileSystem)
        {
            return yaml.Deserialize<Dictionary<long, SdeType>>(sdeFileSystem.TypesYaml);
        }

        protected override void Report(PendingOrDone status)
            => progress.ParseTypes.Report(status);
    }

    public interface IParseData
    {
        public SdeData Parse();
    }

    public class ParseData : IParseData
    {
        private readonly IDownloadSde downloadSde;
        private readonly IParseNames parseNames;
        private readonly IParseTypes parseTypes;

        public ParseData(IDownloadSde downloadSde, IParseNames parseNames, IParseTypes parseTypes)
        {
            this.downloadSde = downloadSde;
            this.parseNames = parseNames;
            this.parseTypes = parseTypes;
        }

        public SdeData Parse()
        {
            Task<Dictionary<long, string>> names = Tasks.LongRunning(ParseNames);
            Task<Dictionary<long, SdeType>> types = Tasks.LongRunning(ParseTypes);

            Task.WaitAll([names, types]);

            return new()
            {
                Version = downloadSde.Download().LastWriteTimeUtc,
                Names = names.Result,
                Types = types.Result,
            };
        }

        private Dictionary<long, string> ParseNames()
            => parseNames.Parse();

        private Dictionary<long, SdeType> ParseTypes()
            => parseTypes.Parse();
    }

    public static class DataParserServices
    {
        public static IServiceCollection AddDataParsers(this IServiceCollection services)
        {
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IParseNames, ParseNames>();
            services.TryAddSingleton<IParseTypes, ParseTypes>();
            services.TryAddSingleton<IParseData, ParseData>();

            return services;
        }
    }
}
