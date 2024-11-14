using Fasciculus.Algorithms;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using Fasciculus.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve
{
    public class Program
    {
        private static FileInfo DataFile
            => EveAssetsDirectories.ResourcesDirectory.File("EveData.dat.gz");

        private static FileInfo UniverseFile
            => EveAssetsDirectories.ResourcesDirectory.File("EveUniverse.dat.gz");

        private static FileInfo NavigationFile
            => EveAssetsDirectories.ResourcesDirectory.File("EveNavigation.dat.gz");

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
                List<string> changes = [];
                Progress progress = new();

                SdeData sdeData = CreateSde(progress);

                EveData eveData = CreateDataFile(sdeData, changes, progress);
                EveUniverse eveUniverse = CreateUniverseFile(sdeData, changes, progress);

                CreateNavigationFile(eveUniverse, changes, progress);

                Console.WriteLine("changes");
                changes.ForEach(change => Console.WriteLine($"  {change}"));
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

        private static EveData CreateDataFile(SdeData sdeData, List<string> changes, IProgress<string> progress)
        {
            EveData eveData = ConvertData.Execute(sdeData, progress);
            using MemoryStream uncompressed = new();

            eveData.Write(uncompressed);
            CreateFile(uncompressed, DataFile, changes, progress);

            return eveData;
        }

        private static EveUniverse CreateUniverseFile(SdeData sdeData, List<string> changes, IProgress<string> progress)
        {
            SdeUniverse sdeUniverse = ParseUniverse.Execute(progress);

            sdeUniverse.Populate(sdeData);

            EveUniverse eveUniverse = ConvertUniverse.Execute(sdeUniverse);
            using MemoryStream uncompressed = new();

            eveUniverse.Write(uncompressed);
            CreateFile(uncompressed, UniverseFile, changes, progress);

            return eveUniverse;
        }

        private static void CreateNavigationFile(IEveUniverse eveUniverse, List<string> changes, IProgress<string> progress)
        {
            EveNavigation navigation = CreateNavigation.Execute(eveUniverse, progress);

            using MemoryStream uncompressed = new();

            navigation.Write(uncompressed);
            CreateFile(uncompressed, NavigationFile, changes, progress);
        }

        private static void CreateFile(MemoryStream uncompressed, FileInfo file, List<string> changes, IProgress<string> progress)
        {
            bool done = false;

            progress.Report($"compressing {file.Name}");

            if (file.Exists)
            {
                if (Equality.AreEqual(uncompressed.ToArray(), GZip.Extract(file)))
                {
                    progress.Report($"  no changes for {file.Name}");
                    done = true;
                }
            }

            if (!done)
            {
                uncompressed.Position = 0;
                GZip.Compress(uncompressed, file);
                changes.Add(file.Name);
            }

            progress.Report($"compressing {file.Name} done");
        }
    }
}