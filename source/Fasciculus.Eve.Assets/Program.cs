using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using Fasciculus.IO;
using System;
using System.IO;

namespace Fasciculus.Eve
{
    public class Program
    {
        private static FileInfo UniverseFile
            => EveAssetsDirectories.ResourcesDirectory.File("EveUniverse.dat");

        private static FileInfo NavigationFile
            => EveAssetsDirectories.ResourcesDirectory.File("Navigation.dat.gz");

        private class Progress : IProgress<string>
        {
            public void Report(string message)
            {
                Console.WriteLine(message);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                Progress progress = new();

                SdeData sdeData = CreateSde(progress);
                EveUniverse eveUniverse = CreateUniverseFile(sdeData, progress);

                CreateNavigationFile(eveUniverse, progress);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static SdeData CreateSde(IProgress<string> progress)
        {
            DownloadSdeZip.Execute(progress);
            ExtractSdeZip.Execute(progress);

            return ParseData.Execute(progress);
        }

        private static EveUniverse CreateUniverseFile(SdeData sdeData, IProgress<string> progress)
        {
            SdeUniverse sdeUniverse = ParseUniverse.Execute(progress);

            sdeUniverse.Populate(sdeData);

            EveUniverse eveUniverse = ConvertUniverse.Execute(sdeUniverse);

            UniverseFile.DeleteIfExists();
            UniverseFile.Write(stream => eveUniverse.Write(new(stream)));

            return eveUniverse;
        }

        private static void CreateNavigationFile(IEveUniverse eveUniverse, IProgress<string> progress)
        {
            EveNavigation navigation = CreateNavigation.Execute(eveUniverse, progress);

            using MemoryStream uncompressed = new MemoryStream();

            navigation.Write(uncompressed);
            uncompressed.Position = 0;
            GZip.Compress(uncompressed, NavigationFile);
        }
    }
}
