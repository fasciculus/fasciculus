using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Threading;
using Fasciculus.Threading;
using System.Collections.ObjectModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class SkillsPageModel : MainThreadObservable
    {
        private readonly ISkillInfoProvider skillsProvider;

        [ObservableProperty]
        public partial bool Loading { get; private set; }

        public ObservableCollection<ISkillInfo> Skills { get; } = [];

        public SkillsPageModel(ISkillInfoProvider skillsProvider)
        {
            this.skillsProvider = skillsProvider;

            Loading = true;

            Skills.CollectionChanged += OnSkillsChanged;
        }

        private void OnSkillsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Loading = Skills.Count == 0;
        }

        public void OnLoaded()
        {
            if (Loading)
            {
                Tasks.LongRunning(AddSkills);
            }
        }

        private void AddSkills()
        {
            Threads.RunInMainThread(() => { skillsProvider.Skills.Apply(Skills.Add); });
        }
    }
}
