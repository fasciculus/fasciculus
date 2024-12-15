using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
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
            return MainThread.InvokeOnMainThreadAsync(() => InternalGoTo(url));
        }

        private static Task InternalGoTo(string url)
        {
            return Shell.Current.GoToAsync(url)
                .ContinueWith(_ => Tasks.Sleep(250));
        }
    }
}
