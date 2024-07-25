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

        public static FileInfo StargatesFile
            => Constants.ResourcesDirectory.File("Stargates.dat");

        public static void Convert()
        {
            Console.WriteLine("ConvertUniverse Started");

            Constants.UniverseEveDirectory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertRegion(d)))
                .WaitAll();

            RegionsFile.Write(stream => Regions.Write(new Data(stream)));
            ConstellationsFile.Write(stream => Constellations.Write(new Data(stream)));
            SolarSystemsFile.Write(stream => SolarSystems.Write(new Data(stream)));
            StargatesFile.Write(stream => Stargates.Write(new Data(stream)));

            Console.WriteLine("ConvertUniverse Done");
        }

        private static void ConvertRegion(DirectoryInfo directory)
        {
            Region region = new(Yaml.Deserialize<SdeRegion>(directory.File("region.yaml")));

            directory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertConstellation(d, region.Id)))
                .WaitAll();

            Console.WriteLine($"Region '{directory.Name}' converted");
        }

        private static void ConvertConstellation(DirectoryInfo directory, int regionId)
        {
            Constellation constellation = new(Yaml.Deserialize<SdeConstellation>(directory.File("constellation.yaml")), regionId);

            directory
                .GetDirectories()
                .Select(d => Task.Run(() => ConvertSolarSystem(d, constellation.Id)))
                .WaitAll();
        }

        private static void ConvertSolarSystem(DirectoryInfo directory, int constellationId)
        {
            SolarSystem.Create(Yaml.Deserialize<SdeSolarSystem>(directory.File("solarsystem.yaml")), constellationId);
        }
    }
}
