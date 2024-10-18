using Fasciculus.IO;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ExtractSdeZip
    {
        public static async Task Execute(IProgress<string> progress)
        {
            progress.Report("extracting sde.zip");
            await Zip.Extract(EveAssetsFiles.SdeZipFile, EveAssetsDirectories.Sde, Zip.Overwrite.Changed);
            progress.Report("extracting sde.zip done");
        }
    }
}
