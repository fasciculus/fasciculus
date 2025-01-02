using Fasciculus.ApiDoc.Models;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.ApiDoc
{
    public class ApiDocumentsBuilder
    {
        private readonly List<FileInfo> projectFiles = [];

        public ApiDocumentsBuilder AddProjectFile(FileInfo projectFile)
        {
            projectFiles.Add(projectFile);

            return this;
        }

        public ApiDocuments Build()
        {
            using ApiWorkspace workspace = new();

            projectFiles.Apply(workspace.AddProjectFile);

            ApiPackage[] packages = ApiParser.Parse(workspace.Projects);
            ApiPackages mergedPackages = ApiMerger.Merge(packages);

            return new()
            {
                Packages = mergedPackages
            };
        }
    }
}
