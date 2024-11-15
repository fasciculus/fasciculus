
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

            GetStations(universe, args[0], out EveNpcStation origin, out EveNpcStation destination);

            Console.WriteLine($"Origin     : {origin.Name}");
            Console.WriteLine($"Destination: {destination.Name}");

#if !DEBUG
            Console.ReadLine();
#endif
        }

        private static void GetStations(EveUniverse universe, string start, out EveNpcStation origin, out EveNpcStation destination)
        {
            EveNpcStation jita = universe.SolarSystems["Jita"]
                .Planets[EveCelestialIndex.Create(4)]
                .Moons[EveCelestialIndex.Create(4)]
                .NpcStations.Last();

            EveNpcStation dodixie = universe.SolarSystems["Dodixie"]
                .Planets[EveCelestialIndex.Create(9)]
                .Moons[EveCelestialIndex.Create(20)]
                .NpcStations.Last();

            origin = start == "Jita" ? jita : dodixie;
            destination = start == "Jita" ? dodixie : jita;
        }
    }
}