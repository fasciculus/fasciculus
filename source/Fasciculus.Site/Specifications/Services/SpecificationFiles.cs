using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationFiles : IEnumerable<FileInfo>
    {
        private readonly List<FileInfo> files;

        public SpecificationFiles()
        {
            FileInfo solutionFile = FileSearch.Search("fasciculus.sln", SearchPath.WorkingDirectoryAndParents).First();

            FileInfo specificationFile = solutionFile.Directory!
                .Combine("source", "Fasciculus.Mathematics", "Properties", "Specifications")
                .File("FixedPoint.md");

            files = [specificationFile];
        }

        public IEnumerator<FileInfo> GetEnumerator()
            => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => files.GetEnumerator();
    }
}
