﻿using System.IO;

namespace Fasciculus.Eve
{
    public static class EveAssetsFiles
    {
        public static FileInfo SdeZipFile => EveAssetsDirectories.DownloadsDirectory.File("sde.zip");
    }
}
