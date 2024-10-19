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

            Task<SdeNames> parseNames = Task.Run(() => ParseNames(progress));

            Task[] tasks = [parseNames];

            tasks.WaitAll();

            progress.Report("parsing data done");

            SdeNames names = parseNames.Result;

            return new(names);
        }

        private static SdeNames ParseNames(IProgress<string> progress)
        {
            progress.Report("  parsing names");

            SdeNames result = new(Yaml.Deserialize<List<SdeName>>(NamesFile));

            progress.Report("  parsing names done");

            return result;
        }
    }
}
