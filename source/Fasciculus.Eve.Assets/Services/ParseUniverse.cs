using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseUniverse
    {
        public Task<SdeRegion[]> Regions { get; }
    }

    public class ParseUniverse : IParseUniverse
    {
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsProgress progress;

        private SdeRegion[]? regions = null;
        private readonly TaskSafeMutex regionsMutex = new();

        public Task<SdeRegion[]> Regions => GetRegions();

        public ParseUniverse(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        private async Task<SdeRegion[]> GetRegions()
        {
            using Locker locker = Locker.Lock(regionsMutex);

            if (regions is null)
            {
                SdeFiles sdeFiles = await extractSde.Files;
                DirectoryInfo[] regionDirectories = sdeFiles.Regions;

                Begin(regionDirectories);
                regions = regionDirectories.AsParallel().Select(ParseRegion).ToArray();
                End();
            }

            return regions;
        }

        private SdeRegion ParseRegion(DirectoryInfo regionDirectory)
        {
            FileInfo file = regionDirectory.File("region.yaml");
            SdeRegion region = yaml.Deserialize<SdeRegion>(file);

            region.Constellations = regionDirectory.GetDirectories().Select(ParseConstellation).ToArray();
            progress.ParseRegions.Report(1);

            return region;
        }

        private SdeConstellation ParseConstellation(DirectoryInfo constellationDirectory)
        {
            FileInfo file = constellationDirectory.File("constellation.yaml");
            SdeConstellation constellation = yaml.Deserialize<SdeConstellation>(file);

            constellation.SolarSystems = constellationDirectory.GetDirectories().Select(ParseSolarSystem).ToArray();
            progress.ParseConstellations.Report(1);

            return constellation;
        }

        private SdeSolarSystem ParseSolarSystem(DirectoryInfo solarSystemDirectory)
        {
            FileInfo file = solarSystemDirectory.File("solarsystem.yaml");
            SdeSolarSystem solarSystem = yaml.Deserialize<SdeSolarSystem>(file);

            progress.ParseSolarSystems.Report(1);

            return solarSystem;
        }

        private DirectoryInfo[] Begin(DirectoryInfo[] regionDirectories)
        {
            DirectoryInfo[] constellationDirectories = regionDirectories.SelectMany(d => d.GetDirectories()).ToArray();
            long solarSystemDirectories = constellationDirectories.SelectMany(d => d.GetDirectories()).Count();

            progress.ParseRegions.Begin(regionDirectories.Length);
            progress.ParseConstellations.Begin(constellationDirectories.Length);
            progress.ParseSolarSystems.Begin(solarSystemDirectories);

            return regionDirectories;
        }

        private void End()
        {
            progress.ParseRegions.End();
            progress.ParseConstellations.End();
            progress.ParseSolarSystems.End();
        }
    }

    public static class ParseUniverseServices
    {
        public static IServiceCollection AddParseUniverse(this IServiceCollection services)
        {
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IParseUniverse, ParseUniverse>();

            return services;
        }
    }
}
