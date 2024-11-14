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

            EveNames names = ConvertNames(sdeData.Names);
            EveNpcCorporations npcCorporations = ConvertNpcCorporations(sdeData.NpcCorporations);

            progress.Report("converting data done");

            return new()
            {
                Names = names,
                NpcCorporations = npcCorporations
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
    }
}
