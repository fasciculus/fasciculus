using Fasciculus.Eve.Models;
using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Fasciculus.Eve.Services
{
    public interface IEveSettings
    {

    }

    public class EveSettings : IEveSettings
    {
        private static readonly EveSettingsContext settingsContext = new();

        private readonly IEveFileSystem fileSystem;
        private readonly EveCombinedSettings settings;

        public EveSettings(IEveFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            settings = Read(fileSystem.UserSettings);
            settings.Planets.PropertyChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Write(fileSystem.UserSettings, settings);
        }

        private static EveCombinedSettings Read(FileInfo file)
        {
            if (file.Exists)
            {
                using Stream stream = file.OpenRead();

                object? obj = JsonSerializer.Deserialize(stream, typeof(EveCombinedSettings), settingsContext);

                if (obj is not null && obj is EveCombinedSettings settings)
                {
                    return settings;
                }
            }

            return new();
        }

        private static void Write(FileInfo file, EveCombinedSettings settings)
        {
            using Stream stream = file.Create();

            JsonSerializer.Serialize(stream, settings, typeof(EveCombinedSettings), settingsContext);
        }
    }
}
