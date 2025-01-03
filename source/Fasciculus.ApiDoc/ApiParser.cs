using Fasciculus.ApiDoc.Models;
using Fasciculus.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Fasciculus.ApiDoc
{
    public class ApiParser
    {
        private ApiPackage apiPackage = new() { Name = string.Empty };
        private ApiNamespaces apiNamespaces = [];

        private ApiPackage[] ParseProjects(IEnumerable<Project> projects)
            => [.. projects.Select(ParseProject)];

        private ApiPackage ParseProject(Project project)
        {
            string targetFramework = ParseTargetFramework(project);

            apiPackage = new() { Name = project.AssemblyName };
            apiNamespaces = apiPackage.Namespaces;
            apiPackage.TargetFrameworks.Add(targetFramework);

            Document document = project.Documents.First();

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                ParseSyntaxTrees(compilation.SyntaxTrees);
            }

            return apiPackage;
        }

        private void ParseSyntaxTrees(ImmutableArray<SyntaxTree> syntaxTrees)
            => syntaxTrees.Apply(ParseSyntaxTree);

        private void ParseSyntaxTree(SyntaxTree syntaxTree)
        {
            if (IsGenerated(syntaxTree))
            {
                return;
            }

            if (syntaxTree.TryGetRoot(out SyntaxNode? root))
            {
                if (root is CompilationUnitSyntax compilationUnit)
                {
                    ParseCompilationUnit(compilationUnit);
                }
            }
        }

        private static readonly string[] GeneratedFileNameEndings = [".designer", ".generated", ".g", ".g.i"];

        private static bool IsGenerated(SyntaxTree syntaxTree)
        {
            FileInfo file = new(syntaxTree.FilePath);
            string name = file.Name;

            if (name.StartsWith("TemporaryGeneratedFile_", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string extension = file.Extension;

            name = name[..^extension.Length];

            return GeneratedFileNameEndings.Any(s => name.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        private void ParseCompilationUnit(CompilationUnitSyntax compilationUnit)
        {
            foreach (MemberDeclarationSyntax member in compilationUnit.Members)
            {
                if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    string name = namespaceDeclaration.Name.ToString();

                    apiNamespaces.Add(name);
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
            ApiParser apiParser = new();

            return apiParser.ParseProjects(projects);
        }
    }
}
