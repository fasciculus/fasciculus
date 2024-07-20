using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using Fasciculus.Eve.Utilities;
using Fasciculus.IO;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertRegions
    {
        public static FileInfo[] SourceFiles
            => Constants.UniverseEveDirectory.GetFiles("region.yaml", SearchOption.AllDirectories);

        public static FileInfo TargetFile
            => Constants.ResourcesDirectory.File("Regions.dat");

        public static void Convert()
        {
            FileInfo[] sourceFiles = SourceFiles;
            FileInfo? newestSourceFile = sourceFiles.MaxBy(f => f.LastWriteTimeUtc);

            if (newestSourceFile != null && newestSourceFile.IsNewerThan(TargetFile))
            {
                Console.WriteLine("ConvertRegions");

                sourceFiles.AsParallel()
                    .Select(Yaml.Deserialize<SdeRegion>).AsSequential()
                    .Select(s => new Region(s)).ToList()
                    .ForEach(Regions.Add);

                TargetFile.Write(stream => Regions.Write(new Data(stream)));
            }
        }
    }
}
