using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadRegions
    {
        public static async Task LoadAsync()
        {
            DirectoryInfo eveDirectory = Constants.UniverseDirectory.Combine("eve");
            IEnumerable<FileInfo> regionFiles = eveDirectory.EnumerateFiles("region.yaml", SearchOption.AllDirectories);
            List<Task<Region>> tasks = new();

            foreach (FileInfo regionFile in regionFiles)
            {
                tasks.Add(LoadRegion.LoadAsync(regionFile));
            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<Region> task in tasks)
            {
                Regions.Add(task.Result);
            }

            await Task.Delay(0);
        }
    }
}
