using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve
{
    public class Program
    {
        private class Progress : IProgress<string>
        {
            public void Report(string message)
            {
                Console.WriteLine(message);
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                Progress progress = new();

                await DownloadSdeZip.Execute(progress);
                await ExtractSdeZip.Execute(progress);

                SdeData data = ParseData.Execute(progress);
                SdeUniverse universe = ParseUniverse.Execute(progress);

                universe.Populate(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
