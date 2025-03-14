using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class PackageFiles : Files
    {
        public PackageFiles(ContentFiles contentFiles)
            : base(contentFiles, "Packages") { }

        protected override string GetKey(FileInfo file)
            => file.NameWithoutExtension();
    }
}
