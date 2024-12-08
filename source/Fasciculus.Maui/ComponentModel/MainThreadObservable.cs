using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
using System.ComponentModel;

namespace Fasciculus.Maui.ComponentModel
{
    public class MainThreadObservable : ObservableObject
    {
        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                base.OnPropertyChanging(e);
            }));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                base.OnPropertyChanged(e);
            }));
        }
    }
}
