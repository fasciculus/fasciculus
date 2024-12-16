using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Fasciculus.Eve.Services
{
    public interface ISkillManager : ISkillInfoProvider
    {
    }

    public class SkillManager : MainThreadObservable, ISkillManager
    {
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            IndentSize = 2,
            WriteIndented = true,
        };

        private readonly IEveFileSystem fileSystem;
        private readonly EveSkillInfos skills;

        public IEnumerable<ISkillInfo> Skills => skills.Skills;
        IEnumerable<IMutableSkill> IMutableSkillProvider.Skills => skills.Skills;
        IEnumerable<ISkill> ISkillProvider.Skills => skills.Skills;

        public SkillManager(IEveFileSystem fileSystem, IDataProvider data)
        {
            this.fileSystem = fileSystem;

            skills = LoadSkills(fileSystem.SkillSettings, GetSkillConsumers(data));
            skills.PropertyChanged += OnSkillsChanged;
        }

        private void OnSkillsChanged(object? sender, PropertyChangedEventArgs e)
            => SaveSkills();

        private void SaveSkills()
        {
            Dictionary<int, int> data = skills.Skills.ToDictionary(x => x.Type.Id, x => x.Level);
            using Stream stream = fileSystem.SkillSettings.Create();

            JsonSerializer.Serialize(stream, data, serializerOptions);
        }

        private static EveSkillInfos LoadSkills(FileInfo file, IEnumerable<ISkillConsumer> skillConsumers)
        {
            EveSkillInfos skills = new(skillConsumers);

            if (file.Exists)
            {
                try
                {
                    using Stream stream = file.OpenRead();
                    Dictionary<int, int>? data = JsonSerializer.Deserialize<Dictionary<int, int>>(stream);

                    data?.Apply(x => { skills.Set(x.Key, x.Value); });
                }
                catch { }
            }

            return skills;
        }

        public bool Fulfills(IEnumerable<ISkill> requiredSkills)
            => skills.Fulfills(requiredSkills);

        private static IEnumerable<ISkillConsumer> GetSkillConsumers(IDataProvider data)
            => data.Blueprints.Select(x => x.Manufacturing);
    }
}
