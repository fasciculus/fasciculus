using Fasciculus.Eve.Models;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Fasciculus.Eve.Services
{
    public interface IEveSettings
    {
        public EveIndustrySettings IndustrySettings { get; }
        public EvePlanetsSettings PlanetsSettings { get; }
    }

    public class EveSettings : IEveSettings
    {
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            IndentSize = 2,
            WriteIndented = true,
        };

        private readonly IEveFileSystem fileSystem;
        private readonly EveCombinedSettings settings;

        public EveIndustrySettings IndustrySettings => settings.Industry;
        public EvePlanetsSettings PlanetsSettings => settings.Planets;

        public EveSettings(IEveFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            settings = Read(fileSystem.UserSettings);
            settings.Industry.PropertyChanged += OnSettingsChanged;
            settings.Planets.PropertyChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Write(fileSystem.UserSettings, settings);
        }

        private static EveCombinedSettings Read(FileInfo file)
        {
            EveCombinedSettings? settings = null;

            if (file.Exists)
            {
                using Stream stream = file.OpenRead();

                settings = JsonSerializer.Deserialize<EveCombinedSettings>(stream);
            }

            return settings ?? new();
        }

        private static void Write(FileInfo file, EveCombinedSettings settings)
        {
            using Stream stream = file.Create();

            JsonSerializer.Serialize(stream, settings, serializerOptions);
        }
    }
}
