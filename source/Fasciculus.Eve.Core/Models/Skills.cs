using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Maui.ComponentModel;
using System.Collections.Generic;
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

    public interface IMutableSkill : ISkill
    {
        public new int Level { get; set; }
    }

    public interface ISkillAffected
    {
        public bool IsAffectedBySkill(EveType skillType);
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

    public class EveSkills
    {
        private readonly EveSkill[] skills;
        private readonly Dictionary<EveType, EveSkill> byType;

        public EveSkills(IEnumerable<EveSkill> skills)
        {
            this.skills = skills.ToArray();

            byType = this.skills.ToDictionary(x => x.Type);
        }

        public bool Fulfills(IEnumerable<EveSkill> requirements)
            => requirements.All(Fulfills);

        private bool Fulfills(EveSkill requirement)
            => byType.TryGetValue(requirement.Type, out EveSkill? skill) && skill.Level >= requirement.Level;
    }

    public partial class EveMutableSkill : MainThreadObservable, IMutableSkill
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
                UpdateCrements();
            }
        }

        [RelayCommand]
        private void Increment()
        {
            if (Level < 5)
            {
                ++Level;
                UpdateCrements();
            }
        }
    }

    public partial class EveSkillInfo : EveMutableSkill
    {
        public int Affects { get; }

        public EveSkillInfo(EveType type, int level, IEnumerable<ISkillAffected> affecteds)
            : base(type, level)
        {
            Affects = affecteds.Where(x => x.IsAffectedBySkill(type)).Count();
        }
    }
}
