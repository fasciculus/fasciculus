using Fasciculus.Eve.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertData
    {
        public static EveData Execute(SdeData sdeData, IProgress<string> progress)
        {
            progress.Report("converting data");

            EveMarketGroups marketGroups = ConvertMarketGroups(sdeData.MarketGroups);
            EveNames names = ConvertNames(sdeData.Names);
            EveNpcCorporations npcCorporations = ConvertNpcCorporations(sdeData.NpcCorporations);
            EveStationOperations stationOperations = ConvertStationOperations(sdeData.StationOperations);
            EveTypes types = ConvertTypes(sdeData.Types);

            progress.Report("converting data done");

            return new(marketGroups, names, npcCorporations, stationOperations, types);
        }

        private static EveMarketGroups ConvertMarketGroups(Dictionary<int, SdeMarketGroup> sdeMarketGroups)
            => new(sdeMarketGroups.Select(kvp => ConvertMarketGroup(kvp.Key, kvp.Value)).ToArray());

        private static EveMarketGroup ConvertMarketGroup(int rawId, SdeMarketGroup sdeMarketGroup)
        {
            EveId id = EveId.Create(rawId);
            EveId parentId = EveId.Create(sdeMarketGroup.ParentGroupID);
            string name = sdeMarketGroup.NameID.En;

            return new(id, parentId)
            {
                Name = name
            };
        }

        private static EveNames ConvertNames(SdeNames names)
            => new(names.Names.ToDictionary(kvp => EveId.Create(kvp.Key), kvp => kvp.Value));

        private static EveNpcCorporations ConvertNpcCorporations(Dictionary<int, SdeNpcCorporation> sdeNpcCorporations)
            => new(sdeNpcCorporations.Select(kvp => ConvertNpcCorporation(kvp.Key, kvp.Value)).ToArray());

        private static EveNpcCorporation ConvertNpcCorporation(int rawId, SdeNpcCorporation sdeNpcCorporation)
        {
            EveId id = EveId.Create(rawId);
            string name = sdeNpcCorporation.NameId.En;

            return new(id, name);
        }

        private static EveStationOperations ConvertStationOperations(Dictionary<int, SdeStationOperation> sdeStationOperation)
            => new(sdeStationOperation.Select(kvp => ConvertStationOperation(kvp.Key, kvp.Value)).ToArray());

        private static EveStationOperation ConvertStationOperation(int rawId, SdeStationOperation sdeStationOperation)
        {
            EveId id = EveId.Create(rawId);
            string name = sdeStationOperation.OperationNameID.En;

            return new(id, name);
        }

        private static EveTypes ConvertTypes(Dictionary<int, SdeType> types)
            => new(types.Select(kvp => ConvertType(kvp.Key, kvp.Value)).ToArray());

        private static EveType ConvertType(int rawId, SdeType sdeType)
        {
            EveId id = EveId.Create(rawId);
            EveId groupId = EveId.Create(sdeType.GroupId);
            EveId marketGroupId = EveId.Create(sdeType.MarketGroupID);
            string name = sdeType.Name.En;
            int portionSize = sdeType.PortionSize;
            bool published = sdeType.Published;
            double volume = sdeType.Volume;

            return new EveType(id, groupId, marketGroupId)
            {
                Name = name,
                PortionSize = portionSize,
                Published = published,
                Volume = volume
            };
        }
    }
}
