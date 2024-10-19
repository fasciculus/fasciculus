using Fasciculus.Eve.Models;
using System;

namespace Fasciculus.Eve.Operations
{
    public static class ParseUniverse
    {
        public static SdeUniverse Execute(IProgress<string> progress)
        {
            progress.Report("parsing universe");
            progress.Report("parsing universe done");

            return new();
        }
    }
}
