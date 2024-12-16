using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Services
{
    public interface ISkillManager : ISkillInfoProvider
    {
    }

    public class SkillManager : MainThreadObservable, ISkillManager
    {
        private readonly EveSkillInfos skills;

        public IEnumerable<ISkillInfo> Skills => skills.Skills;
        IEnumerable<IMutableSkill> IMutableSkillProvider.Skills => skills.Skills;
        IEnumerable<ISkill> ISkillProvider.Skills => skills.Skills;

        public SkillManager(IDataProvider data)
        {
            skills = new(GetSkillConsumers(data));
        }

        private static IEnumerable<ISkillConsumer> GetSkillConsumers(IDataProvider data)
            => data.Blueprints.Select(x => x.Manufacturing);

        public bool Fulfills(IEnumerable<ISkill> requiredSkills)
            => skills.Fulfills(requiredSkills);
    }
}
