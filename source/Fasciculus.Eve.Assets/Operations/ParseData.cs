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

        private static FileInfo TypesFile
            => EveAssetsDirectories.FsdDirectory.File("types.yaml");

        public static SdeData Execute(IProgress<string> progress)
        {
            progress.Report("parsing data");

            Task<SdeNames> names = Task.Run(() => ParseNames(progress));
            Task<SdeStationOperations> stationOperations = Task.Run(() => ParseStationOperations(progress));
            Task<SdeTypes> types = Task.Run(() => ParseTypes(progress));

            Task.WaitAll([names, stationOperations, types]);

            progress.Report("parsing data done");

            return new()
            {
                Names = names.Result,
                StationOperations = stationOperations.Result,
                Types = types.Result,
            };
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

        private static SdeTypes ParseTypes(IProgress<string> progress)
        {
            progress.Report("  parsing types");

            SdeTypes result = new(Yaml.Deserialize<Dictionary<int, SdeType>>(TypesFile));

            progress.Report("  parsing types done");

            return result;
        }
    }
}
