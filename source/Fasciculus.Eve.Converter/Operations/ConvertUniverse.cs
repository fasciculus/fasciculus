using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using Fasciculus.Eve.Utilities;
using Fasciculus.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertUniverse
    {
        public static FileInfo RegionsFile
            => Constants.ResourcesDirectory.File("Regions.dat");

        public static FileInfo ConstellationsFile
            => Constants.ResourcesDirectory.File("Constellations.dat");

        public static FileInfo SolarSystemsFile
            => Constants.ResourcesDirectory.File("SolarSystems.dat");

        private struct ConstellationResult
        {
            public Constellation constellation;
            public SolarSystem[] solarSystems;
        }

        private struct RegionResult
        {
            public Region region;
            public ConstellationResult[] constellations;
        }

        public static void Convert()
        {
            Console.WriteLine("ConvertUniverse Started");

            Task<RegionResult>[] tasks = Constants.UniverseEveDirectory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertRegion(d)))
                .ToArray();

            Task.WaitAll(tasks);

            RegionResult[] regions = tasks.Select(t => t.Result).ToArray();

            foreach (RegionResult region in regions)
            {
                Regions.Add(region.region);

                foreach (ConstellationResult constellation in region.constellations)
                {
                    Constellations.Add(constellation.constellation);

                    foreach (SolarSystem solarSystem in constellation.solarSystems)
                    {
                        SolarSystems.Add(solarSystem);
                    }
                }
            }

            RegionsFile.Write(stream => Regions.Write(new Data(stream)));
            ConstellationsFile.Write(stream => Constellations.Write(new Data(stream)));
            SolarSystemsFile.Write(stream => SolarSystems.Write(new Data(stream)));

            Console.WriteLine("ConvertUniverse Done");
        }

        private static RegionResult ConvertRegion(DirectoryInfo directory)
        {
            Region region = new(Yaml.Deserialize<SdeRegion>(directory.File("region.yaml")));

            Task<ConstellationResult>[] tasks = directory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertConstellation(d, region.Id)))
                .ToArray();

            Task.WaitAll(tasks);

            Console.WriteLine($"Region '{directory.Name}' converted");

            return new()
            {
                region = region,
                constellations = tasks.Select(t => t.Result).ToArray()
            };
        }

        private static ConstellationResult ConvertConstellation(DirectoryInfo directory, int regionId)
        {
            Constellation constellation = new(Yaml.Deserialize<SdeConstellation>(directory.File("constellation.yaml")), regionId);

            Task<SolarSystem>[] tasks = directory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertSolarSystem(d, constellation.Id)))
                .ToArray();

            Task.WaitAll(tasks);

            return new()
            {
                constellation = constellation,
                solarSystems = tasks.Select(t => t.Result).ToArray()
            };
        }

        private static SolarSystem ConvertSolarSystem(DirectoryInfo directory, int constellationId)
        {
            return new(Yaml.Deserialize<SdeSolarSystem>(directory.File("solarsystem.yaml")), constellationId);
        }
    }
}
