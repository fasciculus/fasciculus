using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Fasciculus.IO
{
    public static class Folders
    {
        public static DirectoryInfo Documents
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return new(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                }

                throw new NotImplementedException();
            }
        }
    }
}
