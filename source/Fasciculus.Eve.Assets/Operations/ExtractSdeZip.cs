using Fasciculus.IO;
using System;

namespace Fasciculus.Eve.Operations
{
    public static class ExtractSdeZip
    {
        public static void Execute(IProgress<string> progress)
        {
            progress.Report("extracting sde.zip");
            Zip.Extract(EveAssetsFiles.SdeZipFile, EveAssetsDirectories.SdeDirectory, Zip.Overwrite.Changed);
            progress.Report("extracting sde.zip done");
        }
    }
}
