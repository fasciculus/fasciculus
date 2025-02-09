using Fasciculus.Docs.Content.Services;
using Fasciculus.Site.Specifications.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationContent
    {
        private readonly Dictionary<string, SpecificationEntry> entries;

        public SpecificationContent(SpecificationFiles files, SpecificationCompiler compiler)
        {
            entries = files.GetFiles().Select(compiler.Compile).ToDictionary(e => e.Id);
        }

        public SpecificationEntry GetEntry(string id)
        {
            return entries[id];
        }
    }
}
