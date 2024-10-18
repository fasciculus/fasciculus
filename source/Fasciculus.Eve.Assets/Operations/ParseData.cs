using Fasciculus.Eve.Models;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ParseData
    {
        public static async Task<SdeData> Execute(IProgress<string> progress)
        {
            progress.Report("parsing data");
            progress.Report("parsing data done");

            return new SdeData();
        }
    }
}
