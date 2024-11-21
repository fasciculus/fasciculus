using System.IO;

namespace Fasciculus.Eve.IO
{
    public static class EveAssetsFiles
    {
        public static FileInfo SdeZipFile => EveAssetsDirectories.DownloadsDirectory.File("sde.zip");
    }
}
