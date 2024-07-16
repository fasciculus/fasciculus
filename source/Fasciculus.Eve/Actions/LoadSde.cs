using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadSde
    {
        public static async Task LoadAsync()
        {
            await ExtractSde.RunAsync();

            List<Task> tasks = new()
            {
                Task.Run(() => LoadNames.LoadAsync()),
                Task.Run(() => LoadUniverse.LoadAsync())
            };

            Task.WaitAll(tasks.ToArray());

            await Task.Delay(0);
        }
    }
}
