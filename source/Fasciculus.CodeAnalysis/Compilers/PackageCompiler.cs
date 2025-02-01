using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal class PackageCompiler
    {
        private readonly CompilerContext context;

        public PackageCompiler(CompilerContext context)
        {
            this.context = context;
        }

        public PackageSymbol Compile()
        {
            ParsedProject project = context.Project;

            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.AssemblyName);
            UriPath link = new(name);
            TargetFramework framework = context.Framework;
            CompilationUnitCompiler compiler = new(context);
            FileInfo commentFile = context.CommentsDirectory.File(context.Project.AssemblyName + ".xml");
            SymbolComment comment = SymbolComment.FromFile(context.CommentContext, commentFile);
            Uri repository = context.Project.Repository;
            CompilationUnitInfo[] compilationUnits = [.. roots.Select(compiler.Compile)];

            return new(name, framework, comment, compilationUnits)
            {
                Name = name,
                Link = link,
                Modifiers = PackageSymbol.PackageModifiers,
                Repository = repository
            };
        }
    }
}
