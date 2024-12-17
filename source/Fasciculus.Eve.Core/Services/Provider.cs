using Fasciculus.Eve.Models;
using System;

namespace Fasciculus.Eve.Services
{
    public interface IEveProvider : IDataProvider, IUniverseProvider, INavigationProvider
    {
    }

    public class EveProvider : IEveProvider
    {
        private readonly IDataProvider data;
        private readonly IUniverseProvider universe;
        private readonly INavigationProvider navigation;

        public DateTime Version => data.Version;
        public EveTypes Types => data.Types;
        public EveStationOperations StationOperations => data.StationOperations;
        public EveNpcCorporations NpcCorporations => data.NpcCorporations;
        public EvePlanetSchematics PlanetSchematics => data.PlanetSchematics;
        public EveBlueprints Blueprints => data.Blueprints;

        public EveRegions Regions => universe.Regions;
        public EveConstellations Constellations => universe.Constellations;
        public EveSolarSystems SolarSystems => universe.SolarSystems;
        public EveAllPlanets Planets => universe.Planets;
        public EveAllMoons Moons => universe.Moons;
        public EveStations Stations => universe.Stations;
        public EveStargates Stargates => universe.Stargates;

        public EveNavigation Navigation => navigation.Navigation;

        public EveProvider(IDataProvider data, IUniverseProvider universe, INavigationProvider navigation)
        {
            this.data = data;
            this.universe = universe;
            this.navigation = navigation;
        }
    }
}
