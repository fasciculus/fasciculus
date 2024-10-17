using Fasciculus.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class ExtractSdeZip
    {
        public static async Task Execute()
        {
            await Zip.Extract(EveAssetsFiles.SdeZipFile, EveAssetsDirectories.Sde, Zip.Overwrite.Changed);
        }
    }
}
