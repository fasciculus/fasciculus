using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class NamespaceCompiler : CSharpSyntaxWalker
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFramework framework;

        private readonly Stack<NamespaceList> namespacesStack = [];

        public NamespaceCompiler(TargetFramework framework)
        {
            this.framework = framework;
        }

        public NamespaceList Compile(NamespaceDeclarationSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            namespacesStack.Push(new());
            AddNamespace(node);

            return namespacesStack.Pop();
        }

        private void AddNamespace(NamespaceDeclarationSyntax node)
        {
            SymbolName name = new(node.Name.ToString());
            NamespaceSymbol @namespace = new(name, framework);
            NamespaceList namespaces = namespacesStack.Peek();

            namespaces.AddOrMergeWith(@namespace);
        }
    }
}
