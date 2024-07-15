using Fasciculus.Eve.Actions;
using System.Threading.Tasks;

namespace Fasciculus.Eve;

public class Program
{
    public static async Task Main(string[] args)
    {
        await ExtractSde.RunAsync();
        await LoadNames.RunAsync();
    }
}
