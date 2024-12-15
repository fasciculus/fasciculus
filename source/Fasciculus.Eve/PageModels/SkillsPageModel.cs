using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class SkillsPageModel : MainThreadObservable
    {
        public EveSkillInfo[] Skills { get; }

        public SkillsPageModel(IEveResources resources)
        {
            Skills = GetSkills(resources);
        }

        private static EveSkillInfo[] GetSkills(IEveResources resources)
        {
            EveTypes types = Tasks.Wait(resources.Data).Types;
            EveBlueprints blueprints = Tasks.Wait(resources.Data).Blueprints;
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
