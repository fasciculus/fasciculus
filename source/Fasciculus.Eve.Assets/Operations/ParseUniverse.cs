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

            SdeRegions regions = ParseRegions(progress);

            progress.Report("parsing universe done");

            return new(regions);
        }

        private static SdeRegions ParseRegions(IProgress<string> progress)
        {
            IEnumerable<SdeRegion> regions = EveAssetsDirectories.RegionsDirectory.GetDirectories()
                .Select(directory => Task.Run(() => ParseRegion(directory, progress)))
                .WaitAll();

            return new(regions);
        }

        private static SdeRegion ParseRegion(DirectoryInfo directory, IProgress<string> progress)
        {
            FileInfo file = directory.File("region.yaml");

            return Yaml.Deserialize<SdeRegion>(file);
        }
    }
}
