using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseUniverse
    {
        public Task<SdeRegion[]> ParseAsync();
    }

    public class ParseUniverse : IParseUniverse
    {
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsProgress progress;

        private SdeRegion[]? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        public ParseUniverse(IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        public async Task<SdeRegion[]> ParseAsync()
        {
            using Locker locker = Locker.Lock(resultMutex);

            await Task.Yield();

            if (result is null)
            {
                ISdeFileSystem sdeFileSystem = extractSde.Extract();
                DirectoryInfo[] regionDirectories = sdeFileSystem.Regions;

                Begin(regionDirectories);

                Task<SdeRegion>[] tasks = regionDirectories.Select(ParseRegionAsync).ToArray();

                result = Tasks.WaitAll(tasks).Select(t => t.Result).ToArray();

                End();
            }

            return result;
        }

        private async Task<SdeRegion> ParseRegionAsync(DirectoryInfo regionDirectory)
        {
            await Task.Yield();

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
