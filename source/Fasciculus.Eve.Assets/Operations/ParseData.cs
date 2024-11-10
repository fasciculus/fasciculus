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

        private static FileInfo StationOperationsFile
            => EveAssetsDirectories.FsdDirectory.File("stationOperations.yaml");

        public static SdeData Execute(IProgress<string> progress)
        {
            progress.Report("parsing data");

            Task<SdeNames> parseNames = Task.Run(() => ParseNames(progress));
            Task<SdeStationOperations> parseStationOperations = Task.Run(() => ParseStationOperations(progress));

            Task.WaitAll([parseNames, parseStationOperations]);

            SdeNames names = parseNames.Result;
            SdeStationOperations stationOperations = parseStationOperations.Result;

            progress.Report("parsing data done");

            return new(names, stationOperations);
        }

        private static SdeNames ParseNames(IProgress<string> progress)
        {
            progress.Report("  parsing names");

            SdeNames result = new(Yaml.Deserialize<List<SdeName>>(NamesFile));

            progress.Report("  parsing names done");

            return result;
        }

        private static SdeStationOperations ParseStationOperations(IProgress<string> progress)
        {
            progress.Report("  parsing station operations");

            SdeStationOperations result = new(Yaml.Deserialize<Dictionary<int, SdeStationOperation>>(StationOperationsFile));

            progress.Report("  parsing station operations done");

            return result;
        }
    }
}
