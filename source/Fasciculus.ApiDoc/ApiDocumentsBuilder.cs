using Fasciculus.ApiDoc.Models;
using Fasciculus.ApiDoc.Parsers;
using Fasciculus.ApiDoc.Support;
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

            ApiPackages packages = ProjectsParser.Parse(workspace.Projects);

            return new()
            {
                Packages = packages
            };
        }
    }
}
