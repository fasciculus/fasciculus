using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
        private readonly IEveResourcesProgress progress;
        private readonly INavigator navigator;

        [ObservableProperty]
        private string dataText = "Pending";

        [ObservableProperty]
        private Color dataColor = Colors.Orange;

        [ObservableProperty]
        private string universeText = "Pending";

        [ObservableProperty]
        private Color universeColor = Colors.Orange;

        [ObservableProperty]
        private string navigationText = "Pending";

        [ObservableProperty]
        private Color navigationColor = Colors.Orange;

        [ObservableProperty]
        private string timerText = string.Empty;

        private readonly Stopwatch stopwatch = new();

        public LoadingPageModel(IEveResources resources, IEveResourcesProgress progress, INavigator navigator)
        {
            this.resources = resources;
            this.progress = progress;
            this.navigator = navigator;

            this.progress.PropertyChanged += OnProgressChanged;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(2000)))
                .ContinueWith(GoToMainPage);
        }

        private void LoadResources()
        {
            IDispatcherTimer timer = Application.Current!.Dispatcher.CreateTimer();

            stopwatch.Start();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += OnTimerTick;
            timer.Start();

            Task<EveData> data = resources.Data;
            Task<EveUniverse> universe = resources.Universe;
            Task<EveNavigation> navigation = resources.Navigation;

            Task.WaitAll([data, universe, navigation]);

            timer.Stop();
            stopwatch.Stop();
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            DataText = progress.Data ? "Done" : "Pending";
            DataColor = progress.Data ? Colors.Green : Colors.Orange;

            UniverseText = progress.Universe ? "Done" : "Pending";
            UniverseColor = progress.Universe ? Colors.Green : Colors.Orange;

            NavigationText = progress.Navigation ? "Done" : "Pending";
            NavigationColor = progress.Navigation ? Colors.Green : Colors.Orange;
        }

        private void OnTimerTick(object? sender, EventArgs ev)
        {
            double elapsed = stopwatch.ElapsedMilliseconds / 1000.0;

            TimerText = elapsed.ToString("0.0") + " s";
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
