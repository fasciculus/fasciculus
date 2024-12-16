using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public interface ISkill
    {
        public EveType Type { get; }
        public string Name { get; }
        public int Level { get; }

        public bool CanDecrement { get; }
        public bool CanIncrement { get; }
    }

    public interface IMutableSkill : ISkill, INotifyPropertyChanged
    {
        public new int Level { get; set; }

        public IRelayCommand DecrementCommand { get; }
        public IRelayCommand IncrementCommand { get; }
    }

    public interface ISkillInfo : IMutableSkill
    {
        public int Affects { get; }
    }

    public interface ISkillProvider : INotifyPropertyChanged
    {
        public IEnumerable<ISkill> Skills { get; }

        public bool Fulfills(IEnumerable<ISkill> requiredSkills);
    }

    public interface IMutableSkillProvider : ISkillProvider
    {
        public new IEnumerable<IMutableSkill> Skills { get; }
    }

    public interface ISkillInfoProvider : IMutableSkillProvider
    {
        public new IEnumerable<ISkillInfo> Skills { get; }
    }

    public interface ISkillConsumer
    {
        public IEnumerable<ISkill> RequiredSkills { get; }

        public bool RequiresSkill(EveType skillType);
    }

    public class EveSkill : ISkill
    {
        public class Data
        {
            public int Id { get; }
            public int Level { get; }

            public Data(int id, int level)
            {
                Id = id;
                Level = level;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Level = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(Level);
            }
        }

        private readonly Data data;

        public EveType Type { get; }
        public string Name => Type.Name;
        public int Level => data.Level;

        public bool CanDecrement => false;
        public bool CanIncrement => false;

        public EveSkill(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Id];
        }
    }

    public class EveSkills : ObservableObject, ISkillProvider
    {
        private readonly EveSkill[] skills;
        private readonly Dictionary<EveType, EveSkill> byType;

        public IEnumerable<ISkill> Skills => skills;

        public EveSkills(IEnumerable<EveSkill> skills)
        {
            this.skills = skills.ToArray();
            this.skills.Apply(x => { });

            byType = this.skills.ToDictionary(x => x.Type);
        }

        public bool Fulfills(IEnumerable<ISkill> requiredSkills)
            => requiredSkills.All(Fulfills);

        private bool Fulfills(ISkill requiredSkill)
            => byType.TryGetValue(requiredSkill.Type, out EveSkill? skill) && skill.Level >= requiredSkill.Level;
    }

    public partial class EveMutableSkill : ObservableObject, IMutableSkill
    {
        public EveType Type { get; }
        public string Name => Type.Name;

        [ObservableProperty]
        private int level;

        [ObservableProperty]
        private bool canDecrement;

        [ObservableProperty]
        private bool canIncrement;

        public EveMutableSkill(EveType type, int level)
        {
            Type = type;
            Level = level;

            UpdateCrements();

            PropertyChanged += OnThisPropertyChanged;
        }

        private void OnThisPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (nameof(Level) == e.PropertyName)
            {
                UpdateCrements();
            }
        }

        private void UpdateCrements()
        {
            CanDecrement = Level > 0;
            CanIncrement = Level < 5;
        }

        [RelayCommand]
        private void Decrement()
        {
            if (Level > 0)
            {
                --Level;
            }
        }

        [RelayCommand]
        private void Increment()
        {
            if (Level < 5)
            {
                ++Level;
            }
        }
    }

    public partial class EveSkillInfo : EveMutableSkill, ISkillInfo
    {
        public int Affects { get; }

        public EveSkillInfo(EveType type, int level, IEnumerable<ISkillConsumer> consumers)
            : base(type, level)
        {
            Affects = consumers.Where(x => x.RequiresSkill(type)).Count();
        }
    }

    public class EveSkillInfos : ObservableObject, ISkillInfoProvider
    {
        private readonly ISkillInfo[] skills;
        private readonly Dictionary<EveType, ISkillInfo> byType;
        private readonly Dictionary<int, ISkillInfo> byTypeId;

        public IEnumerable<ISkillInfo> Skills => skills;
        IEnumerable<IMutableSkill> IMutableSkillProvider.Skills => skills;
        IEnumerable<ISkill> ISkillProvider.Skills => skills;

        public EveSkillInfos(IEnumerable<ISkillConsumer> skillConsumers)
        {
            skills = [.. CreateSkills(skillConsumers)];
            skills.Apply(x => { x.PropertyChanged += OnSkillChanged; });

            byType = skills.ToDictionary(x => x.Type);
            byTypeId = skills.ToDictionary(x => x.Type.Id);
        }

        private void OnSkillChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (nameof(ISkillInfo.Level) == e.PropertyName)
            {
                OnPropertyChanged(nameof(Skills));
            }
        }

        private static IEnumerable<EveSkillInfo> CreateSkills(IEnumerable<ISkillConsumer> skillConsumers)
        {
            return skillConsumers
                .SelectMany(x => x.RequiredSkills)
                .Select(x => x.Type)
                .Distinct()
                .OrderBy(x => x.Name)
                .Select(x => new EveSkillInfo(x, 0, skillConsumers));
        }

        public bool Fulfills(IEnumerable<ISkill> requiredSkills)
            => requiredSkills.All(Fulfills);

        private bool Fulfills(ISkill requiredSkill)
            => byType.TryGetValue(requiredSkill.Type, out ISkillInfo? skill) && skill.Level >= requiredSkill.Level;

        public void Set(int typeId, int level)
        {
            if (byTypeId.TryGetValue(typeId, out ISkillInfo? skillInfo))
            {
                level = Math.Max(0, Math.Min(5, level));

                skillInfo.Level = level;
            }
        }
    }
}
