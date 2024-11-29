using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Threading;
using System.ComponentModel;

namespace Fasciculus.Models
{
    public abstract class MainThreadObservable : ObservableObject
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Threads.RunInMainThread(() => { base.OnPropertyChanged(e); });
        }

        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            Threads.RunInMainThread(() => { base.OnPropertyChanging(e); });
        }
    }
}
