﻿using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class NamespaceCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
        [
            SyntaxKind.ClassDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.QualifiedName,
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFrameworks frameworks;

        private readonly ClassCompiler classCompiler;

        private UriPath link = new();
        private ClassList classes = new();

        public NamespaceCompiler(TargetFramework framework)
            : base(AcceptedKinds)
        {
            frameworks = new TargetFrameworks(framework);

            classCompiler = new(framework);
        }

        public NamespaceSymbol Compile(NamespaceDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            SymbolName name = new(node.Name.ToString());

            link = parentLink.Append(name.Name);
            classes = new();

            DefaultVisit(node);

            return new(name, link, frameworks, classes);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            classes.AddOrMergeWith(classCompiler.Compile(node, link));
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            //base.VisitInterfaceDeclaration(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            //base.VisitEnumDeclaration(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node) { }
    }
}
