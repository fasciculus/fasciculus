using Fasciculus.Eve.Actions;
using Fasciculus.Eve.Models;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await ExtractSde.RunAsync();
            await LoadNames.LoadAsync();
            await LoadRegions.LoadAsync();

            // Console.WriteLine(Names.Get(30000142));
            Region region = Regions.Get(10000016);

            Console.WriteLine(region.Name);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
