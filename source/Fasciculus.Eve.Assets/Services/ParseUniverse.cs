using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IUniverseParser
    {
        public SdeRegion[] Parse();
    }

    public class UniverseParser : IUniverseParser
    {
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsDirectories assetsDirectories;

        private readonly ILongProgress regionsProgress;
        private readonly ILongProgress constellationsProgress;
        private readonly ILongProgress solarSystemsProgress;

        private SdeRegion[]? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        public UniverseParser(IAssetsDirectories assetsDirectories, IExtractSde extractSde, IYaml yaml,
            [FromKeyedServices(ServiceKeys.RegionsParser)] ILongProgress regionsProgress,
            [FromKeyedServices(ServiceKeys.ConstellationsParser)] ILongProgress constellationsProgress,
            [FromKeyedServices(ServiceKeys.SolarSystemsParser)] ILongProgress solarSystemsProgress)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.assetsDirectories = assetsDirectories;

            this.regionsProgress = regionsProgress;
            this.constellationsProgress = constellationsProgress;
            this.solarSystemsProgress = solarSystemsProgress;
        }

        public SdeRegion[] Parse()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                extractSde.Extract();

                DirectoryInfo[] regionDirectories = Start();

                result = regionDirectories.AsParallel().Select(ParseRegion).ToArray();

                Done();
            }

            return result;
        }

        private SdeRegion ParseRegion(DirectoryInfo regionDirectory)
        {
            FileInfo file = regionDirectory.File("region.yaml");
            SdeRegion region = yaml.Deserialize<SdeRegion>(file);
            DirectoryInfo[] constellationDirectories = regionDirectory.GetDirectories();

            region.Constellations = constellationDirectories.Select(ParseConstellation).ToArray();

            regionsProgress.Report(1);

            return region;
        }

        private SdeConstellation ParseConstellation(DirectoryInfo constellationDirectory)
        {
            FileInfo file = constellationDirectory.File("constellation.yaml");
            SdeConstellation constellation = yaml.Deserialize<SdeConstellation>(file);
            DirectoryInfo[] solarSystemDirectories = constellationDirectory.GetDirectories();

            constellation.SolarSystems = solarSystemDirectories.Select(ParseSolarSystem).ToArray();

            constellationsProgress.Report(1);

            return constellation;
        }

        private SdeSolarSystem ParseSolarSystem(DirectoryInfo solarSystemDirectory)
        {
            FileInfo file = solarSystemDirectory.File("solarsystem.yaml");
            SdeSolarSystem solarSystem = yaml.Deserialize<SdeSolarSystem>(file);

            solarSystemsProgress.Report(1);

            return solarSystem;
        }

        private DirectoryInfo[] Start()
        {
            DirectoryInfo[] regionDirectories = assetsDirectories.UniverseEve.GetDirectories();
            DirectoryInfo[] constellationDirectories = regionDirectories.SelectMany(d => d.GetDirectories()).ToArray();
            DirectoryInfo[] solarSystemDirectories = constellationDirectories.SelectMany(d => d.GetDirectories()).ToArray();

            regionsProgress.Start(regionDirectories.Length);
            constellationsProgress.Start(constellationDirectories.Length);
            solarSystemsProgress.Start(solarSystemDirectories.Length);

            return regionDirectories;
        }

        private void Done()
        {
            regionsProgress.Done();
            constellationsProgress.Done();
            solarSystemsProgress.Done();
        }
    }

    public static class UniverseParserServices
    {
        public static IServiceCollection AddUniverseParser(this IServiceCollection services)
        {
            services.AddAssetsFileSystem();
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IUniverseParser, UniverseParser>();

            return services;
        }
    }
}
