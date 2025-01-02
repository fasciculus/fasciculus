using Fasciculus.Threading;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.ApiDoc.Support
{
    public class ApiWorkspace : IDisposable
    {
        private readonly MSBuildWorkspace workspace;

        public IEnumerable<Project> Projects
            => workspace.CurrentSolution.Projects.Where(project => project.HasDocuments);

        public ApiWorkspace()
        {
            Initialize();

            workspace = MSBuildWorkspace.Create();
            workspace.SkipUnrecognizedProjects = true;
        }

        ~ApiWorkspace()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            workspace.Dispose();
        }

        public void AddProjectFile(FileInfo projectFile)
        {
            if (!HasProjectFile(projectFile))
            {
                Tasks.Wait(workspace.OpenProjectAsync(projectFile.FullName));
            }
        }

        public bool HasProjectFile(FileInfo projectFile)
        {
            // workspace.CurrentSolution.Projects.Any(project => projectFile.FullName == project.FilePath);

            Project[] projects = [.. workspace.CurrentSolution.Projects];

            foreach (var project in projects)
            {
                if (project.FilePath == projectFile.FullName)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Initialize()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                VisualStudioInstance[] instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

                if (instances.Length > 0)
                {
                    VisualStudioInstance instance = instances.OrderByDescending(x => x.Version).First();

                    MSBuildLocator.RegisterInstance(instance);
                }
                else
                {
                    MSBuildLocator.RegisterDefaults();
                }
            }
        }
    }
}
