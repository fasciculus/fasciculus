using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class SpecificationFiles : Files
    {
        public SpecificationFiles(ContentFiles contentFiles)
            : base(contentFiles, "Specifications") { }

        protected override string GetKey(FileInfo file)
        {
            string name = file.NameWithoutExtension();
            string package = file.Directory!.Name;

            return $"{package}|{name}";
        }
    }
}
