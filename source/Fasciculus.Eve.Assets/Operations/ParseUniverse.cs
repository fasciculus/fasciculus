using Fasciculus.Eve.Models;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ParseUniverse
    {
        public static async Task<SdeUniverse> Execute(IProgress<string> progress)
        {
            await Task.CompletedTask;

            progress.Report("parsing universe");
            progress.Report("parsing universe done");

            return new SdeUniverse();
        }
    }
}
