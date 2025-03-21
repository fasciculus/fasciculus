using Fasciculus.Collections;
using Fasciculus.Eve.Models;
using Fasciculus.Mathematics.Matrices;
using Fasciculus.Mathematics.Vectors;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;

namespace Fasciculus.Eve.Assets.Services
{
    public interface ICreateNavigation
    {
        public Task<EveNavigation.Data> Navigation { get; }
    }

    public class CreateNavigation : ICreateNavigation
    {
        private static readonly SortedSet<string> ALLOWED_REGIONS =
            [
                // Caldari
                "Black Rise", "Lonetrek", "The Citadel", "The Forge",
                // Gallente
                "Essence", "Everyshore", "Placid", "Sinq Laison", "Solitude", "Verge Vendor", 
                // Amarr
                "Aridia", "Devoid", "Domain", "Kador", "Khanid", "Kor-Azor", "Tash-Murkon", "The Bleak Lands",
                // Minmatar
                "Derelik", "Heimatar", "Metropolis", "Molden Heath",
                // Other
                "Genesis"
            ];

        private readonly IConvertUniverse convertUniverse;
        private readonly IAssetsProgress progress;

        private EveUniverse.Data? universe = null;
        private readonly TaskSafeMutex universeMutex = new();

        private EveSolarSystem.Data[]? systems = null;
        private readonly TaskSafeMutex systemsMutex = new();

        private Dictionary<uint, uint>? stargateSystems = null;
        private readonly TaskSafeMutex stargatesSystemsMutex = new();

        private Dictionary<uint, uint[]>? neighbours = null;
        private readonly TaskSafeMutex neighboursMutex = new();

        private Dictionary<uint, double>? systemSecurities = null;
        private readonly TaskSafeMutex systemSecuritiesMutex = new();

        private SparseBoolVector? allowedSystems = null;
        private readonly TaskSafeMutex allowedSystemsMutex = new();

        private SparseBoolVector? forbiddenSystems = null;
        private readonly TaskSafeMutex forbiddenSystemsMutex = new();

        private Dictionary<EveSecurity.Level, SparseBoolMatrix>? connections = null;
        private readonly TaskSafeMutex connectionsMutex = new();

        private EveNavigation.Data? navigation = null;
        private readonly TaskSafeMutex navigationMutex = new();

        public Task<EveNavigation.Data> Navigation => GetNavigationAsync();

        public CreateNavigation(IConvertUniverse convertUniverse, IAssetsProgress progress)
        {
            this.convertUniverse = convertUniverse;
            this.progress = progress;
        }

        private Task<EveNavigation.Data> GetNavigationAsync()
        {
            GetUniverse();

            return Tasks.Start(GetNavigation, true);
        }

        private EveNavigation.Data GetNavigation()
        {
            using Locker locker = Locker.Lock(navigationMutex);

            if (navigation is null)
            {
                progress.CreateDistances.Begin(EveSecurity.Levels.Length * GetSystems().Length);

                Task<SparseShortMatrix>[] tasks = EveSecurity.Levels
                    .Select(x => Tasks.Start(() => GetDistances(x), true))
                    .ToArray();

                Task.WaitAll(tasks);
                navigation = new(tasks.Select(x => x.Result));

                progress.CreateDistances.End();
            }

            return navigation;
        }

        private SparseShortMatrix GetDistances(EveSecurity.Level level)
        {
            SparseBoolMatrix connections = GetConnections()[level];

            Tuple<uint, SparseShortVector>[] rows = GetSystems()
                .AsParallel()
                .Select(s => Tuple.Create(s.Id, GetDistances(s, connections)))
                .ToArray();

            SparseShortMatrix result = new(rows.ToDictionary());

            return result;
        }

        private SparseShortVector GetDistances(EveSolarSystem.Data solarSystem, SparseBoolMatrix connections)
        {
            SparseShortVector distances = SparseShortVector.Empty;
            SparseBoolVector visited = SparseBoolVector.Empty;
            SparseBoolVector front = new([solarSystem.Id]);
            short distance = 0;

            while (front.Indices.Any())
            {
                ++distance;
                front = connections * front - visited;
                visited += front;
                distances += distance * front;
            }

            progress.CreateDistances.Report(1);

            return distances;
        }

        private Dictionary<EveSecurity.Level, SparseBoolMatrix> GetConnections()
        {
            using Locker locker = Locker.Lock(connectionsMutex);

            if (connections is null)
            {
                progress.CreateConnections.Begin(2);
                progress.CreateConnections.Report(1);

                connections = EveSecurity.Levels
                    .AsParallel()
                    .Select(level => Tuple.Create(level, GetConnections(level)))
                    .ToDictionary(x => x.Item1, x => x.Item2);

                progress.CreateConnections.End();
            }

            return connections;
        }

        private SparseBoolMatrix GetConnections(EveSecurity.Level level)
        {
            Dictionary<uint, double> security = GetSecurity();
            EveSecurity.Filter filter = EveSecurity.Filters[level];
            SparseBoolVector forbidden = GetForbiddenSystems();

            Dictionary<uint, SparseBoolVector> rows = GetSystems()
                .Select(s => Tuple.Create(s.Id, GetConnections(s, security, filter, forbidden)))
                .ToDictionary();

            return new(rows);
        }

        private SparseBoolVector GetConnections(EveSolarSystem.Data system, Dictionary<uint, double> security, EveSecurity.Filter filter,
            SparseBoolVector forbidden)
        {
            SparseBoolVector all = new(GetNeighbours()[system.Id].Where(x => filter(security[x])));

            return all - forbidden;
        }

        private Dictionary<uint, double> GetSecurity()
        {
            using Locker locker = Locker.Lock(systemSecuritiesMutex);

            return systemSecurities ??= GetSystems().ToDictionary(x => x.Id, x => x.Security);
        }

        private Dictionary<uint, uint[]> GetNeighbours()
        {
            using Locker locker = Locker.Lock(neighboursMutex);

            return neighbours ??= GetSystems().Select(GetNeighbours).ToDictionary();
        }

        private Tuple<uint, uint[]> GetNeighbours(EveSolarSystem.Data solarSystem)
        {
            Dictionary<uint, uint> stargateSystems = GetStargateSystems();

            uint[] neighbours = solarSystem.Stargates
                .Select(x => stargateSystems[x.Destination])
                .ToArray();

            return Tuple.Create(solarSystem.Id, neighbours);
        }

        private Dictionary<uint, uint> GetStargateSystems()
        {
            using Locker locker = Locker.Lock(stargatesSystemsMutex);

            EveSolarSystem.Data[] systems = GetSystems();
            SortedSet<uint> allowed = new(systems.Select(s => s.Id));

            return stargateSystems ??= systems.SelectMany(GetStargateSystems).ToDictionary();
        }

        private static IEnumerable<Tuple<uint, uint>> GetStargateSystems(EveSolarSystem.Data solarSystem)
            => solarSystem.Stargates.Select(x => Tuple.Create(x.Id, solarSystem.Id));

        private EveUniverse.Data GetUniverse()
        {
            using Locker locker = Locker.Lock(universeMutex);

            return universe ??= Tasks.Wait(convertUniverse.Universe);
        }

        private EveSolarSystem.Data[] GetSystems()
        {
            using Locker locker = Locker.Lock(systemsMutex);

            return systems ??= GetUniverse().Regions
                .SelectMany(GetSystems).ToArray();
        }

        private IEnumerable<EveSolarSystem.Data> GetSystems(EveRegion.Data region)
            => region.Constellations.SelectMany(GetSystems);

        private IEnumerable<EveSolarSystem.Data> GetSystems(EveConstellation.Data constellation)
            => constellation.SolarSystems;

        private SparseBoolVector GetAllowedSystems()
        {
            using Locker locker = Locker.Lock(allowedSystemsMutex);

            if (allowedSystems is null)
            {
                allowedSystems = new(GetUniverse().Regions
                    .Where(r => ALLOWED_REGIONS.Contains(r.Name))
                    .SelectMany(GetSystems)
                    .Select(s => s.Id));
            }

            return allowedSystems;
        }

        private SparseBoolVector GetForbiddenSystems()
        {
            using Locker locker = Locker.Lock(forbiddenSystemsMutex);

            if (forbiddenSystems is null)
            {
                SparseBoolVector all = new(GetSystems().Select(s => s.Id));

                forbiddenSystems = all - GetAllowedSystems();
            }

            return forbiddenSystems;
        }
    }
}
