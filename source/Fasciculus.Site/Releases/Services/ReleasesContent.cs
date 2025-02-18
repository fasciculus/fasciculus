using Fasciculus.Docs.Content.Services;
using Fasciculus.Site.Releases.Models;
using System.Linq;

namespace Fasciculus.Site.Releases.Services
{
    public class ReleasesContent
    {
        private readonly Roadmap roadmap;

        public ReleasesContent(RoadmapFiles files, RoadmapCompiler roadmapCompiler)
        {
            roadmap = roadmapCompiler.Compile(files.GetKeys().First());
        }

        public Roadmap GetRoadmap()
        {
            return roadmap;
        }
    }
}
