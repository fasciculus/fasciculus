using Statiq.App;
using Statiq.Common;
using Statiq.Web;
using System.Threading.Tasks;

namespace Fasciculus.Site
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            int result = await Bootstrapper
                .Factory
                .CreateWeb(args)
                .SetOutputPath(Locations.OutputDirectory.FullName)
                .RunAsync();

            return result;
        }
    }
}
