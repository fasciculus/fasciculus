using Fasciculus.Eve.Models;
using Fasciculus.Mathematics;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface ICreateNavigation
    {
        public Task<EveNavigation.Data> Navigation { get; }
    }

    public class CreateNavigation : ICreateNavigation
    {
        private readonly IConvertUniverse convertUniverse;
        private readonly IAssetsProgress progress;

        private EveUniverse.Data? universe = null;
        private readonly TaskSafeMutex universeMutex = new();

        private EveSolarSystem.Data[]? systems = null;
        private readonly TaskSafeMutex systemsMutex = new();

        private Dictionary<int, int>? stargates = null;
        private readonly TaskSafeMutex stargatesMutex = new();

        private Dictionary<int, int[]>? neighbours = null;
        private readonly TaskSafeMutex neighboursMutex = new();

        private Dictionary<int, double>? security = null;
        private readonly TaskSafeMutex securityMutex = new();

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

            return Tasks.LongRunning(GetNavigation);
        }

        private EveNavigation.Data GetNavigation()
        {
            using Locker locker = Locker.Lock(navigationMutex);

            if (navigation is null)
            {
                progress.CreateDistances.Begin(EveSecurity.Levels.Length * GetSystems().Length);

                SparseShortMatrix[] distances = EveSecurity.Levels.Select(GetDistances).ToArray();

                navigation = new(distances);

                progress.CreateDistances.End();
            }

            return navigation;
        }

        private SparseShortMatrix GetDistances(EveSecurity.Level level)
        {
            SparseBoolMatrix connections = GetConnections()[level];

            Dictionary<int, SparseShortVector> distances = GetSystems().AsParallel()
                .Select(s => Tuple.Create(s.Id, GetDistances(s, connections)))
                .ToDictionary(x => x.Item1, x => x.Item2);

            return new(distances);
        }

        private SparseShortVector GetDistances(EveSolarSystem.Data solarSystem, SparseBoolMatrix connections)
        {
            progress.CreateDistances.Report(1);

            return SparseShortVector.Empty;
        }

        private Dictionary<EveSecurity.Level, SparseBoolMatrix> GetConnections()
        {
            using Locker locker = Locker.Lock(connectionsMutex);

            if (connections is null)
            {
                progress.CreateConnections.Report(PendingToDone.Working);

                connections = EveSecurity.Levels.AsParallel()
                    .Select(level => Tuple.Create(level, GetConnections(level)))
                    .ToDictionary(x => x.Item1, x => x.Item2);

                progress.CreateConnections.Report(PendingToDone.Done);
            }

            return connections;
        }

        private SparseBoolMatrix GetConnections(EveSecurity.Level level)
        {
            Dictionary<int, double> security = GetSecurity();
            EveSecurity.Filter filter = EveSecurity.Filters[level];

            Dictionary<int, SparseBoolVector> rows = GetSystems()
                .Select(s => Tuple.Create(s.Id, GetConnectionsRow(s, security, filter)))
                .ToDictionary(x => x.Item1, x => x.Item2);

            return new(rows);
        }

        private SparseBoolVector GetConnectionsRow(EveSolarSystem.Data system, Dictionary<int, double> security, EveSecurity.Filter filter)
            => SparseBoolVector.Create(GetNeighbours()[system.Id].Where(x => filter(security[x])));

        private Dictionary<int, double> GetSecurity()
        {
            using Locker locker = Locker.Lock(securityMutex);

            return security ??= GetSystems().ToDictionary(x => x.Id, x => x.Security);
        }

        private Dictionary<int, int[]> GetNeighbours()
        {
            using Locker locker = Locker.Lock(neighboursMutex);

            return neighbours ??= GetSystems().Select(GetNeighbours).ToDictionary(x => x.Item1, x => x.Item2);
        }

        private Tuple<int, int[]> GetNeighbours(EveSolarSystem.Data solarSystem)
        {
            Dictionary<int, int> stargates = GetStargates();

            int[] neighbours = solarSystem.Stargates.Select(x => stargates[x.Id]).ToArray();

            return Tuple.Create(solarSystem.Id, neighbours);
        }

        private Dictionary<int, int> GetStargates()
        {
            using Locker locker = Locker.Lock(stargatesMutex);

            return stargates ??= GetSystems().SelectMany(GetStargates).ToDictionary(x => x.Item1, x => x.Item2);
        }

        private IEnumerable<Tuple<int, int>> GetStargates(EveSolarSystem.Data solarSystem)
            => solarSystem.Stargates.Select(x => Tuple.Create(x.Id, solarSystem.Id));

        private EveUniverse.Data GetUniverse()
        {
            using Locker locker = Locker.Lock(universeMutex);

            return universe ??= Tasks.Wait(convertUniverse.Universe);
        }

        private EveSolarSystem.Data[] GetSystems()
        {
            using Locker locker = Locker.Lock(systemsMutex);

            return systems ??= GetUniverse().Regions.SelectMany(GetSystems).ToArray();
        }

        private IEnumerable<EveSolarSystem.Data> GetSystems(EveRegion.Data region)
            => region.Constellations.SelectMany(GetSystems);

        private IEnumerable<EveSolarSystem.Data> GetSystems(EveConstellation.Data constellation)
            => constellation.SolarSystems;
    }

    public static class CreateNanigationServices
    {
        public static IServiceCollection AddCreateNanigation(this IServiceCollection services)
        {
            services.AddConvertUniverse();
            services.AddAssetsProgress();

            services.TryAddSingleton<ICreateNavigation, CreateNavigation>();

            return services;
        }
    }
}
