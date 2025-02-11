using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class NamespaceFiles : Files
    {
        public NamespaceFiles(ContentFiles contentFiles)
            : base(contentFiles, "Namespaces") { }

        protected override string GetKey(FileInfo file)
            => file.NameWithoutExtension();
    }
}
