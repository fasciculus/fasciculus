using Fasciculus.Eve.Operations;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await DownloadSdeZip.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
