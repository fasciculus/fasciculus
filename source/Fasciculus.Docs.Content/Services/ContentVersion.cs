using System;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class ContentVersion
    {
        public DateTime GetVersion()
        {
            FileInfo assemblyFile = new(typeof(ContentVersion).Assembly.Location);

            return assemblyFile.LastWriteTime;
        }
    }
}
