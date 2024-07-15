using Fasciculus.Eve.Actions;
using Fasciculus.Eve.Models;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve;

public class Program
{
    public static async Task Main(string[] args)
    {
        await ExtractSde.RunAsync();
        await LoadNames.RunAsync();

        Console.WriteLine(Names.Get(30000142));
    }
}
