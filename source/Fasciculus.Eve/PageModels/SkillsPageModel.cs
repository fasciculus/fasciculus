using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using System.Collections.ObjectModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class SkillsPageModel : MainThreadObservable
    {
        private readonly ISkillManager skillManager;

        [ObservableProperty]
        private bool loading = true;

        public ObservableCollection<EveSkillInfo> Skills { get; } = [];

        public SkillsPageModel(ISkillManager skillManager)
        {
            this.skillManager = skillManager;
        }

        public void OnLoaded()
        {
            if (Loading)
            {
                Tasks.LongRunning(AddSkills).ContinueWith(_ => { Loading = false; });
            }
        }

        private void AddSkills()
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() => { skillManager.Skills.Apply(Skills.Add); }));
        }
    }
}
