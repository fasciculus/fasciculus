﻿using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseUniverse
    {
        public SdeRegion[] Parse();
    }

    public class ParseUniverse : IParseUniverse
    {
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IAssetsProgress progress;

        private SdeRegion[]? result = null;
        private readonly TaskSafeMutex resultMutex = new();

        public ParseUniverse(IAssetsDirectories assetsDirectories, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        public SdeRegion[] Parse()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                ISdeFileSystem sdeFileSystem = extractSde.Extract();
                DirectoryInfo[] regionDirectories = sdeFileSystem.Regions;

                Begin(regionDirectories);

                result = regionDirectories.AsParallel().Select(ParseRegion).ToArray();

                End();
            }

            return result;
        }

        private SdeRegion ParseRegion(DirectoryInfo regionDirectory)
        {
            FileInfo file = regionDirectory.File("region.yaml");
            SdeRegion region = yaml.Deserialize<SdeRegion>(file);
            DirectoryInfo[] constellationDirectories = regionDirectory.GetDirectories();

            region.Constellations = constellationDirectories.Select(ParseConstellation).ToArray();

            progress.ParseRegions.Report(1);

            return region;
        }

        private SdeConstellation ParseConstellation(DirectoryInfo constellationDirectory)
        {
            FileInfo file = constellationDirectory.File("constellation.yaml");
            SdeConstellation constellation = yaml.Deserialize<SdeConstellation>(file);
            DirectoryInfo[] solarSystemDirectories = constellationDirectory.GetDirectories();

            constellation.SolarSystems = solarSystemDirectories.Select(ParseSolarSystem).ToArray();

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
            DirectoryInfo[] solarSystemDirectories = constellationDirectories.SelectMany(d => d.GetDirectories()).ToArray();

            progress.ParseRegions.Begin(regionDirectories.Length);
            progress.ParseConstellations.Begin(constellationDirectories.Length);
            progress.ParseSolarSystems.Begin(solarSystemDirectories.Length);

            return regionDirectories;
        }

        private void End()
        {
            progress.ParseRegions.End();
            progress.ParseConstellations.End();
            progress.ParseSolarSystems.End();
        }
    }

    public static class UniverseParserServices
    {
        public static IServiceCollection AddUniverseParser(this IServiceCollection services)
        {
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IParseUniverse, ParseUniverse>();

            return services;
        }
    }
}
