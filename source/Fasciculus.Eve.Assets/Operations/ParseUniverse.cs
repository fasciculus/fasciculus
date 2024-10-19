using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ParseUniverse
    {
        public static SdeUniverse Execute(IProgress<string> progress)
        {
            progress.Report("parsing universe");

            IEnumerable<SdeRegion> regions = ParseRegions(progress);

            progress.Report("parsing universe done");

            return new(regions);
        }

        private static IEnumerable<SdeRegion> ParseRegions(IProgress<string> progress)
        {
            return EveAssetsDirectories.RegionsDirectory.GetDirectories()
                .Select(directory => Task.Run(() => ParseRegion(directory, progress)))
                .WaitAll();
        }

        private static SdeRegion ParseRegion(DirectoryInfo directory, IProgress<string> progress)
        {
            FileInfo file = directory.File("region.yaml");
            SdeRegion region = Yaml.Deserialize<SdeRegion>(file);

            region.Constellations = ParseConstellations(directory).ToList();

            progress.Report($"  parsed {directory.Name}");

            return region;
        }

        private static IEnumerable<SdeConstellation> ParseConstellations(DirectoryInfo directory)
        {
            return directory.GetDirectories().Select(d => Task.Run(() => ParseConstellation(d))).WaitAll();
        }

        private static SdeConstellation ParseConstellation(DirectoryInfo directory)
        {
            FileInfo file = directory.File("constellation.yaml");

            return Yaml.Deserialize<SdeConstellation>(file);
        }
    }
}
