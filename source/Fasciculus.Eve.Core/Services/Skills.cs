using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Services
{
    public interface ISkillManager
    {
        public IEnumerable<EveSkillInfo> Skills { get; }
    }

    public class SkillManager : ISkillManager
    {
        public IEnumerable<EveSkillInfo> Skills { get; }

        public SkillManager(IDataProvider data)
        {
            Skills = GetSkills(data.Blueprints);
        }

        private static EveSkillInfo[] GetSkills(EveBlueprints blueprints)
        {
            EveManufacturing[] manufacturings = [.. blueprints.Select(x => x.Manufacturing)];

            return manufacturings
                .SelectMany(x => x.Skills)
                .Select(x => x.Type)
                .Distinct()
                .OrderBy(x => x.Name)
                .Select(x => new EveSkillInfo(x, 0, manufacturings))
                .ToArray();
        }
    }
}
