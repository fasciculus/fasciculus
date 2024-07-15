using System;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadNames
    {
        public static async Task RunAsync()
        {
            FileInfo file = Constants.BsdDirectory.File("invNames.yaml");

            Console.WriteLine(file.Exists);
            await Task.Delay(0);
        }
    }
}
