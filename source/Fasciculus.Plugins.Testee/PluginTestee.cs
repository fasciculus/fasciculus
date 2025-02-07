using System;
using System.IO;

namespace Fasciculus.Plugins.Testee
{
    public class PluginTestee : IPluginTestee
    {
        public DateTime Version
        {
            get
            {
                FileInfo file = new(GetType().Assembly.Location);

                return file.LastWriteTimeUtc;
            }
        }
    }
}
