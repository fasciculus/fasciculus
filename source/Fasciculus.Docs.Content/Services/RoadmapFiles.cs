using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Docs.Content.Services
{
    public class RoadmapFiles : Files
    {
        public RoadmapFiles(ContentFiles contentFiles)
            : base(contentFiles, "Roadmap") { }

        protected override string GetKey(FileInfo file)
            => file.NameWithoutExtension();
    }
}
