using Fasciculus.Maui.Threading;
using Fasciculus.Threading;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Fasciculus.Maui.Services
{
    public interface INavigator
    {
        public Task GoTo(string url);
    }

    public class Navigator : INavigator
    {
        public Task GoTo(string url)
        {
            return Threads.RunInMainThreadAync(() => { InternalGoTo(url); });
        }

        private static Task InternalGoTo(string url)
        {
            return Shell.Current.GoToAsync(url)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(200)));
        }
    }
}
