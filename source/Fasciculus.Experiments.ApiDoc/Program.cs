using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Threading;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Experiments.ApiDoc
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }

    public static class Program
    {
        public static void Main()
        {
            InitializeMSBuild();

            using MSBuildWorkspace workspace = CreateWorkspace();

            OpenProject(workspace, "Fasciculus.Core");

            Project[] allCoreProjects = [.. workspace.CurrentSolution.Projects.Where(project => "Fasciculus.Core" == project.AssemblyName)];
            Project[] coreProjects = [.. allCoreProjects.Where(project => project.HasDocuments)];
            Project coreProject = coreProjects[0];

            if (Tasks.Wait(coreProject.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
                {
                    if (syntaxTree.TryGetRoot(out SyntaxNode? root))
                    {
                        Visit(root);
                    }
                }
            }
        }

        private static void Visit(SyntaxNode node)
        {

        }

        private static MSBuildWorkspace CreateWorkspace()
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            workspace.SkipUnrecognizedProjects = true;

            return workspace;
        }

        private static void OpenProject(MSBuildWorkspace workspace, string name)
        {
            DirectoryInfo directory = DirectorySearch.Search(name, SearchPath.WorkingDirectoryAndParents).First();
            FileInfo file = directory.File($"{name}.csproj");

            Tasks.Wait(workspace.OpenProjectAsync(file.FullName));
        }

        private static void InitializeMSBuild()
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
