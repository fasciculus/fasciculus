using Fasciculus.Eve.Actions;
using System.Threading.Tasks;

namespace Fasciculus.Eve;

public class Program
{
    public static async Task Main(string[] args)
    {
        await ExtractSDE.RunAsync();
        await LoadNames.RunAsync();
    }
}
