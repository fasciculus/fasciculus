
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using Fasciculus.Eve.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Trade
{
    public class ProgramOptions
    {
        public double IskPerType { get; set; } = 1.0;
        public double VolumePerType { get; set; } = 1.0;

        public static readonly Dictionary<string, string> Switches = new()
        {
            { "-i", "IskPerType" },
            { "-v", "VolumePerType" }
        };

    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                IHost host = CreateHost(args);
                ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
                IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();
                ProgramOptions options = configuration.Get<ProgramOptions>() ?? new();

                await host.StartAsync();

                logger.LogInformation($"VolumePerType = {options.VolumePerType}");
                logger.LogInformation($"IskPerType    = {options.IskPerType}");

                await host.StopAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

#if !DEBUG
            Console.ReadLine();
#endif
        }

        private static IHost CreateHost(string[] args)
        {
            HostApplicationBuilder builder = DI.CreateEmptyBuilder();

            builder.Configuration.AddCommandLine(args, ProgramOptions.Switches);

            builder.Logging.AddConsole();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(builder.Services);

            return builder.Build();
        }

        public static void LogServices(IHost host, ILogger logger)
        {
            IServiceCollection serviceCollection = host.Services.GetRequiredService<IServiceCollection>();
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> names = serviceCollection.Select(s => s.ServiceType.Name).NotNull();
            string message = string.Join("\r\n  ", names);

            logger.LogInformation($"SERVICES\r\n  {message}");
        }

        public static async Task OldMain(string[] args)
        {
            try
            {
                Console.WriteLine("Loading Data");

                EveData data = EveResources.ReadData();
                EveUniverse universe = EveResources.ReadUniverse(data);
                Esi esi = new("rhj1", EveFileSystemInfos.EsiCacheFile);

                esi.OnError += OnEsiError;

                GetStations(universe, args, out EveNpcStation origin, out EveNpcStation destination);
                GetLimits(args, out double volumePerType, out double iskPerType);

                Console.WriteLine($"Origin     : {origin.Name} {origin.Moon.Planet.SolarSystem.Constellation.Region.Id.Value}");
                Console.WriteLine($"Destination: {destination.Name}  {destination.Moon.Planet.SolarSystem.Constellation.Region.Id.Value}");
                Console.WriteLine($"Volume/Type: {volumePerType:0.00} m3");
                Console.WriteLine($"Volume/Type: {(iskPerType / 1_000_000):0.0} M");

                TradeOpportunity[] opportunities
                    = await FindTradeOpportunities.FindAsync(esi, data.Types, origin, destination, volumePerType, iskPerType, OnSearchProgress);

                opportunities = opportunities.OrderByDescending(o => o.Margin).ToArray();

                foreach (TradeOpportunity opportunity in opportunities)
                {
                    Console.WriteLine($"{opportunity.Quantity} x {opportunity.Supply.Type.Name}");
                    Console.WriteLine($"  {opportunity.BuyPrice:0.00} -> {opportunity.SellPrice:0.00} = {(opportunity.Margin * 100):0.0} %");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


#if !DEBUG
            Console.ReadLine();
#endif
        }

        private static void OnEsiError(object? sender, Esi.EsiErrorEventArgs e)
        {
            Console.WriteLine($"Esi error: ({e.StatusCode}) {e.Uri}");
        }

        private static void OnSearchProgress(string message)
        {
            Console.WriteLine(message);
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