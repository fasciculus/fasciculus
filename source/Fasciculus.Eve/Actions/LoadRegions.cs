using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadRegions
    {
        public static DirectoryInfo UniverseEveDirectory => Constants.UniverseDirectory.Combine("eve");
        public static FileInfo[] RegionFiles => UniverseEveDirectory.GetFiles("region.yaml", SearchOption.AllDirectories);

        private static Task<Region> CreateTask(FileInfo file)
            => Task.Run(() => LoadRegion.LoadAsync(file));

        public static async Task LoadAsync()
        {
            List<Task<Region>> tasks = RegionFiles.Select(CreateTask).ToList();

            Task.WaitAll(tasks.ToArray());

            tasks.ForEach(task => Regions.Add(task.Result));

            await Task.Delay(0);
        }
    }
}
