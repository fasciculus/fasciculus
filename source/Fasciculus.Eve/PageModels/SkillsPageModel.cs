using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class SkillsPageModel : MainThreadObservable
    {
        public EveSkillInfo[] Skills { get; }

        public SkillsPageModel(IDataProvider data)
        {
            Skills = GetSkills(data);
        }

        private static EveSkillInfo[] GetSkills(IDataProvider data)
        {
            EveTypes types = data.Types;
            EveBlueprints blueprints = data.Blueprints;
            EveManufacturing[] manufacturings = [.. blueprints.Select(x => x.Manufacturing)];

            EveType[] skillTypes = manufacturings
                .SelectMany(x => x.Skills)
                .Select(x => x.Type)
                .Distinct()
                .OrderBy(x => x.Name)
                .ToArray();

            return skillTypes
                .Select(x => new EveSkillInfo(x, 0, manufacturings))
                .ToArray();
        }
    }
}
