using Fasciculus.Eve.Models;
using System;
using System.Linq;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertData
    {
        public static EveData Execute(SdeData sdeData, IProgress<string> progress)
        {
            progress.Report("converting data");

            EveNames names = ConvertNames(sdeData.Names);

            progress.Report("converting data done");

            return new(names);
        }

        private static EveNames ConvertNames(SdeNames names)
            => new(names.Names.ToDictionary(kvp => EveId.Create(kvp.Key), kvp => kvp.Value));
    }
}
