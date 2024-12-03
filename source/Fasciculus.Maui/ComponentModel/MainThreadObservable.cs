using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using System.ComponentModel;

namespace Fasciculus.Maui.ComponentModel
{
    public abstract class MainThreadObservable : ObservableObject
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => { base.OnPropertyChanged(e); });
        }

        protected override void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => { base.OnPropertyChanging(e); });
        }
    }
}
