using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ParseData
    {
        private static FileInfo MarketGroupsFile
            => EveAssetsDirectories.FsdDirectory.File("marketGroups.yaml");

        private static FileInfo NamesFile
            => EveAssetsDirectories.BsdDirectory.File("invNames.yaml");

        private static FileInfo NpcCorporationsFile
            => EveAssetsDirectories.FsdDirectory.File("npcCorporations.yaml");

        private static FileInfo StationOperationsFile
            => EveAssetsDirectories.FsdDirectory.File("stationOperations.yaml");

        private static FileInfo TypesFile
            => EveAssetsDirectories.FsdDirectory.File("types.yaml");

        public static SdeData Execute(IProgress<string> progress)
        {
            progress.Report("parsing data");

            Task<Dictionary<int, SdeMarketGroup>> marketGroups = Task.Run(() => ParseMarketGroups(progress));
            Task<SdeNames> names = Task.Run(() => ParseNames(progress));
            Task<Dictionary<int, SdeNpcCorporation>> npcCorporations = Task.Run(() => ParseNpcCorporations(progress));
            Task<Dictionary<int, SdeStationOperation>> stationOperations = Task.Run(() => ParseStationOperations(progress));
            Task<Dictionary<int, SdeType>> types = Task.Run(() => ParseTypes(progress));

            Task.WaitAll([names, npcCorporations, stationOperations, types]);

            progress.Report("parsing data done");

            return new()
            {
                MarketGroups = marketGroups.Result,
                Names = names.Result,
                NpcCorporations = npcCorporations.Result,
                StationOperations = stationOperations.Result,
                Types = types.Result,
            };
        }

        private static Dictionary<int, SdeMarketGroup> ParseMarketGroups(IProgress<string> progress)
        {
            progress.Report("  parsing market groups");

            Dictionary<int, SdeMarketGroup> result = Yaml.Deserialize<Dictionary<int, SdeMarketGroup>>(MarketGroupsFile);

            progress.Report("  parsing market groups done");

            return result;
        }

        private static SdeNames ParseNames(IProgress<string> progress)
        {
            progress.Report("  parsing names");

            SdeNames result = new(Yaml.Deserialize<SdeName[]>(NamesFile));

            progress.Report("  parsing names done");

            return result;
        }

        private static Dictionary<int, SdeNpcCorporation> ParseNpcCorporations(IProgress<string> progress)
        {
            progress.Report("  parsing NPC corporations");

            Dictionary<int, SdeNpcCorporation> result = Yaml.Deserialize<Dictionary<int, SdeNpcCorporation>>(NpcCorporationsFile);

            progress.Report("  parsing NPC corporations done");

            return result;
        }

        private static Dictionary<int, SdeStationOperation> ParseStationOperations(IProgress<string> progress)
        {
            progress.Report("  parsing station operations");

            Dictionary<int, SdeStationOperation> result = Yaml.Deserialize<Dictionary<int, SdeStationOperation>>(StationOperationsFile);

            progress.Report("  parsing station operations done");

            return result;
        }

        private static Dictionary<int, SdeType> ParseTypes(IProgress<string> progress)
        {
            progress.Report("  parsing types");

            Dictionary<int, SdeType> result = Yaml.Deserialize<Dictionary<int, SdeType>>(TypesFile);

            progress.Report("  parsing types done");

            return result;
        }
    }
}
