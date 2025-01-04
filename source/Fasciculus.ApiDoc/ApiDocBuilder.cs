﻿using Fasciculus.ApiDoc.Models;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Workspaces;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.ApiDoc
{
    public class ApiDocBuilder
    {
        private readonly List<FileInfo> projectFiles = [];

        public ApiDocBuilder AddProjectFile(FileInfo projectFile)
        {
            projectFiles.Add(projectFile);

            return this;
        }

        public ApiDocuments Build()
        {
            using MSBuildWorkspace workspace = LoadWorkspace();

            PackageCollection packages = ProjectParser.Parse(workspace.CurrentSolution.Projects, false);
            ApiPackages apiPackages = new(packages);

            return new()
            {
                Packages = apiPackages
            };
        }

        private MSBuildWorkspace LoadWorkspace()
        {
            MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();

            projectFiles.Apply(f => { workspace.AddProjectFile(f); });

            return workspace;
        }
    }
}