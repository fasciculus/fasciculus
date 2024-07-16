using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadRegion
    {
        public static async Task<Region> LoadAsync(FileInfo file)
        {
            SdeRegion region = await Yaml.DeserializeAsync<SdeRegion>(file);

            return new(region);
        }
    }
}
