using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ParseData
    {
        private static FileInfo NamesFile
            => EveAssetsDirectories.BsdDirectory.File("invNames.yaml");

        public static async Task<SdeData> Execute(IProgress<string> progress)
        {
            await Task.CompletedTask;

            progress.Report("parsing data");

            Task<List<SdeName>> parseNames = ParseNames(progress);

            Task[] tasks = [parseNames];

            tasks.WaitAll();

            progress.Report("parsing data done");

            return new SdeData();
        }

        private static async Task<List<SdeName>> ParseNames(IProgress<string> progress)
        {
            await Task.CompletedTask;

            progress.Report("  parsing names");

            List<SdeName> result = Yaml.Deserialize<List<SdeName>>(NamesFile);

            progress.Report("  parsing names done");

            return result;
        }
    }
}
