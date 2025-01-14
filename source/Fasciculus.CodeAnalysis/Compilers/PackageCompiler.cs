using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class PackageCompiler
    {
        private readonly CompilerContext context;

        public PackageCompiler(CompilerContext context)
        {
            this.context = context;
        }

        public PackageSymbol Compile(ParsedProject project)
        {
            IEnumerable<CompilationUnitSyntax> roots = project
                .Where(t => t.HasCompilationUnitRoot)
                .Select(t => t.GetCompilationUnitRoot());

            SymbolName name = new(project.AssemblyName);
            UriPath link = new(name);

            CompilerContext subContext = context
                .WithFramework(project.Framework)
                .WithPackage(name);

            Compiler compiler = new(Productions.Instance);

            roots.Apply(compiler.Compile);

            return new(name, link, context.Frameworks, [])
            {
                Comment = CreateComment(project),
            };
        }

        private static SymbolComment CreateComment(ParsedProject project)
        {
            FileInfo? file = project.ProjectDirectory?
                .Combine("Properties", "Comments")
                .File(project.AssemblyName + ".xml");

            if (file is not null && file.Exists)
            {
                try
                {
                    return new(XDocument.Load(file.FullName));
                }
                catch { }
            }

            return SymbolComment.Empty;
        }
    }
}
