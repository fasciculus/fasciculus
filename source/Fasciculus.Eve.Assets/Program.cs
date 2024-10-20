using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using System;
using System.IO;
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

                SdeData sdeData = ParseData.Execute(progress);
                SdeUniverse sdeUniverse = ParseUniverse.Execute(progress);

                sdeUniverse.Populate(sdeData);

                EveUniverse eveUniverse = ConvertUniverse.Execute(sdeUniverse);
                FileInfo universeFile = EveAssetsDirectories.ResourcesDirectory.File("EveUniverse.dat");

                universeFile.DeleteIfExists();
                universeFile.Write(stream => eveUniverse.Write(new(stream)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
