
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Resources;
using System;
using System.Linq;

namespace Fasciculus.Eve.Trade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Loading Data");

            EveData data = EveResources.ReadData();
            EveUniverse universe = EveResources.ReadUniverse(data);

            GetStations(universe, args, out EveNpcStation origin, out EveNpcStation destination);
            GetLimits(args, out double volumePerType, out double iskPerType);

            Console.WriteLine($"Origin     : {origin.Name}");
            Console.WriteLine($"Destination: {destination.Name}");
            Console.WriteLine($"Volume/Type: {volumePerType:0.00} m3");
            Console.WriteLine($"Volume/Type: {(iskPerType / 1_000_000):0.0} M");

            TradeOpportunities.Create(data.Types, origin, destination, volumePerType, iskPerType);

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