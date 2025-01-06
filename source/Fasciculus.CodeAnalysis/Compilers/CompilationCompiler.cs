﻿using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilationCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] CompilationCompilerKinds =
        [
            SyntaxKind.NamespaceDeclaration
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly NamespaceCompiler namespaceCompiler;

        private NamespaceList namespaces;

        public CompilationCompiler(TargetFramework framework)
            : base(CompilationCompilerKinds)
        {
            namespaceCompiler = new(framework);
        }

        public CompilationUnit Compile(CompilationUnitSyntax compilationUnit)
        {
            using Locker locker = Locker.Lock(mutex);

            namespaces = new();

            DefaultVisit(compilationUnit);

            return new(namespaces);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            namespaces.AddOrMergeWith(namespaceCompiler.Compile(node));
        }
    }
}
