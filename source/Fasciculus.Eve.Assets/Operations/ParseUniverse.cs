using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ParseUniverse
    {
        public static SdeUniverse Execute(IProgress<string> progress)
        {
            progress.Report("parsing universe");

            SdeRegion[] regions = ParseRegions(progress);

            progress.Report("parsing universe done");

            return new(regions);
        }

        private static SdeRegion[] ParseRegions(IProgress<string> progress)
        {
            return EveAssetsDirectories.RegionsDirectory.GetDirectories().AsParallel()
                .Select(directory => ParseRegion(directory, progress)).ToArray();
        }

        private static SdeRegion ParseRegion(DirectoryInfo directory, IProgress<string> progress)
        {
            FileInfo file = directory.File("region.yaml");
            SdeRegion region = Yaml.Deserialize<SdeRegion>(file);

            region.Constellations = ParseConstellations(directory);

            progress.Report($"  parsed {directory.Name}");

            return region;
        }

        private static SdeConstellation[] ParseConstellations(DirectoryInfo directory)
        {
            return directory.GetDirectories().Select(ParseConstellation).ToArray();
        }

        private static SdeConstellation ParseConstellation(DirectoryInfo directory)
        {
            FileInfo file = directory.File("constellation.yaml");
            SdeConstellation constellation = Yaml.Deserialize<SdeConstellation>(file);

            constellation.SolarSystems = ParseSolarSystems(directory);

            return constellation;
        }

        private static SdeSolarSystem[] ParseSolarSystems(DirectoryInfo directory)
        {
            return directory.GetDirectories().Select(ParseSolarSystem).ToArray();
        }

        private static SdeSolarSystem ParseSolarSystem(DirectoryInfo directory)
        {
            FileInfo file = directory.File("solarsystem.yaml");
            SdeSolarSystem solarSystem = Yaml.Deserialize<SdeSolarSystem>(file);

            return solarSystem;
        }
    }
}
