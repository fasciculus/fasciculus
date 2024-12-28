using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support.Progressing;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.Services
{
    public enum PendingToDone
    {
        Pending,
        Working,
        Done
    }

    public enum DownloadSdeStatus
    {
        Pending,
        Downloading,
        Downloaded,
        NotModified
    }

    public interface IAssetsProgress : INotifyPropertyChanged
    {
        public ProgressBarProgress DownloadSde { get; }
        public ProgressBarProgress ExtractSde { get; }

        public ProgressBarProgress ParseNames { get; }
        public ProgressBarProgress ParseMarketGroups { get; }
        public ProgressBarProgress ParseTypes { get; }
        public ProgressBarProgress ParseStationOperations { get; }
        public ProgressBarProgress ParseNpcCorporations { get; }
        public ProgressBarProgress ParsePlanetSchematics { get; }
        public ProgressBarProgress ParseBlueprints { get; }

        public ProgressBarProgress ParseRegions { get; }
        public ProgressBarProgress ParseConstellations { get; }
        public ProgressBarProgress ParseSolarSystems { get; }

        public ProgressBarProgress ConvertData { get; }
        public ProgressBarProgress ConvertUniverse { get; }

        public ProgressBarProgress CreateConnections { get; }
        public ProgressBarProgress CreateDistances { get; }

        public ProgressBarProgress CopyImages { get; }
        public ProgressBarProgress CreateImages { get; }
    }

    public partial class AssetsProgress : MainThreadObservable, IAssetsProgress
    {
        public ProgressBarProgress DownloadSde { get; }
        public ProgressBarProgress ExtractSde { get; }

        public ProgressBarProgress ParseNames { get; }
        public ProgressBarProgress ParseMarketGroups { get; }
        public ProgressBarProgress ParseTypes { get; }
        public ProgressBarProgress ParseStationOperations { get; }
        public ProgressBarProgress ParseNpcCorporations { get; }
        public ProgressBarProgress ParsePlanetSchematics { get; }
        public ProgressBarProgress ParseBlueprints { get; }

        public ProgressBarProgress ParseRegions { get; }
        public ProgressBarProgress ParseConstellations { get; }
        public ProgressBarProgress ParseSolarSystems { get; }

        public ProgressBarProgress ConvertData { get; }
        public ProgressBarProgress ConvertUniverse { get; }

        public ProgressBarProgress CreateConnections { get; }
        public ProgressBarProgress CreateDistances { get; }

        public ProgressBarProgress CopyImages { get; }
        public ProgressBarProgress CreateImages { get; }

        public AssetsProgress()
        {
            DownloadSde = new();
            ExtractSde = new();

            ParseNames = new();
            ParseMarketGroups = new();
            ParseTypes = new();
            ParseStationOperations = new();
            ParseNpcCorporations = new();
            ParsePlanetSchematics = new();
            ParseBlueprints = new();

            ParseRegions = new();
            ParseConstellations = new();
            ParseSolarSystems = new();

            ConvertData = new();
            ConvertUniverse = new();

            CreateConnections = new();
            CreateDistances = new();

            CopyImages = new();
            CreateImages = new();
        }
    }
}
