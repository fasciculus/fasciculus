using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class BlogFiles : Files
    {
        public BlogFiles(ContentFiles contentFiles)
            : base(contentFiles, "Blog") { }

        protected override string GetKey(FileInfo file)
        {
            string name = file.NameWithoutExtension();
            string month = file.Directory!.Name[1..];
            string year = file.Directory!.Parent!.Name[1..];

            return $"{year}.{month}.{name}";
        }
    }
}
