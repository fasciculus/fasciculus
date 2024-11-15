
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Trade
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Loading Data");

                EveData data = EveResources.ReadData();
                EveUniverse universe = EveResources.ReadUniverse(data);
                Esi esi = new("rhj1", EveFileSystemInfos.EsiCacheFile);

                GetStations(universe, args, out EveNpcStation origin, out EveNpcStation destination);
                GetLimits(args, out double volumePerType, out double iskPerType);

                Console.WriteLine($"Origin     : {origin.Name} {origin.Moon.Planet.SolarSystem.Constellation.Region.Id.Value}");
                Console.WriteLine($"Destination: {destination.Name}  {destination.Moon.Planet.SolarSystem.Constellation.Region.Id.Value}");
                Console.WriteLine($"Volume/Type: {volumePerType:0.00} m3");
                Console.WriteLine($"Volume/Type: {(iskPerType / 1_000_000):0.0} M");

                await TradeOpportunities.CreateAsync(esi, data.Types, origin, destination, volumePerType, iskPerType);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }


#if !DEBUG
            Console.ReadLine();
#endif
        }

        private static void GetStations(EveUniverse universe, string[] args, out EveNpcStation origin, out EveNpcStation destination)
        {
            EveNpcStation jita = universe.SolarSystems["Jita"]
                .Planets[EveCelestialIndex.Create(4)]
                .Moons[EveCelestialIndex.Create(4)]
                .NpcStations.Last();

            EveNpcStation dodixie = universe.SolarSystems["Dodixie"]
                .Planets[EveCelestialIndex.Create(9)]
                .Moons[EveCelestialIndex.Create(20)]
                .NpcStations.Last();

            origin = args[0] == "Jita" ? jita : dodixie;
            destination = args[0] == "Jita" ? dodixie : jita;
        }

        private static void GetLimits(string[] args, out double volumePerType, out double iskPerType)
        {
            volumePerType = double.Parse(args[1]);
            iskPerType = double.Parse(args[2]);
        }
    }
}