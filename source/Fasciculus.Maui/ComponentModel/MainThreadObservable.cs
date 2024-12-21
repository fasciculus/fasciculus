using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.Threading;
using System.ComponentModel;

namespace Fasciculus.Maui.ComponentModel
{
    public class MainThreadObservable : ObservableObject
    {
        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
            => Threads.RunInMainThread(() => { base.OnPropertyChanging(e); });

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
            => Threads.RunInMainThread(() => { base.OnPropertyChanged(e); });
    }
}
