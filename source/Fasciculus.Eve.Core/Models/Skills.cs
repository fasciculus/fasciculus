using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveSkill
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
        public int Level => data.Level;

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
}
