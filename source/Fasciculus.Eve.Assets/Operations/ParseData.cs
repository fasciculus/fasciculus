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

        public static SdeData Execute(IProgress<string> progress)
        {
            progress.Report("parsing data");

            Task<SdeNames> parseNames = ParseNames(progress);

            Task.WaitAll([parseNames]);

            SdeNames names = parseNames.Result;

            progress.Report("parsing data done");

            return new(names);
        }

        private static async Task<SdeNames> ParseNames(IProgress<string> progress)
        {
            await Task.CompletedTask;

            progress.Report("  parsing names");

            SdeNames result = new(Yaml.Deserialize<List<SdeName>>(NamesFile));

            progress.Report("  parsing names done");

            return result;
        }
    }
}
