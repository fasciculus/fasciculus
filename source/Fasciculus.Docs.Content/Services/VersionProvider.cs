using System;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class VersionProvider
    {
        public DateTime GetVersion()
        {
            FileInfo assemblyFile = new(typeof(VersionProvider).Assembly.Location);

            return assemblyFile.LastWriteTime;
        }
    }
}
