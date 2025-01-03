using Fasciculus.ApiDoc.Models;
using Fasciculus.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Fasciculus.ApiDoc
{
    public class ApiParser
    {
        private static ApiPackage[] ParseProjects(IEnumerable<Project> projects)
            => [.. projects.Select(ParseProject)];

        private static ApiPackage ParseProject(Project project)
        {
            string targetFramework = ParseTargetFramework(project);

            ApiPackage package = new()
            {
                Name = project.AssemblyName
            };

            package.TargetFrameworks.Add(targetFramework);

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                ParseSyntaxTrees(compilation.SyntaxTrees, package.Namespaces);
            }

            return package;
        }

        private static void ParseSyntaxTrees(ImmutableArray<SyntaxTree> syntaxTrees, ApiNamespaces namespaces)
            => syntaxTrees.Apply(syntaxTree => { ParseSyntaxTree(syntaxTree, namespaces); });

        private static void ParseSyntaxTree(SyntaxTree syntaxTree, ApiNamespaces namespaces)
        {
            if (IsGenerated(syntaxTree))
            {
                return;
            }

            if (syntaxTree.TryGetRoot(out SyntaxNode? root))
            {
                if (root is CompilationUnitSyntax compilationUnit)
                {
                    ParseCompilationUnit(compilationUnit, namespaces);
                }
            }
        }

        private static bool IsGenerated(SyntaxTree syntaxTree)
        {
            return syntaxTree.FilePath.EndsWith(".g.cs");
        }

        private static void ParseCompilationUnit(CompilationUnitSyntax compilationUnit, ApiNamespaces namespaces)
        {
            foreach (MemberDeclarationSyntax member in compilationUnit.Members)
            {
                if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    string name = namespaceDeclaration.Name.ToString();

                    namespaces.Add(name);
                }
            }
        }

        private static string ParseTargetFramework(Project project)
        {
            string suffix = project.Name[project.AssemblyName.Length..];

            return (suffix.StartsWith('(') && suffix.EndsWith(')')) ? suffix[1..^1] : string.Empty;
        }

        public static ApiPackage[] Parse(IEnumerable<Project> projects)
        {
            return ParseProjects(projects);
        }
    }
}
