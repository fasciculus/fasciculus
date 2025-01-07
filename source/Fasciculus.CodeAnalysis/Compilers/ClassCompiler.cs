using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class ClassCompiler : FilteredCompiler
    {
        private static readonly SyntaxKind[] AcceptedKinds =
        [
        ];

        private readonly TaskSafeMutex mutex = new();

        private readonly TargetFramework framework;

        private UriPath link = new();

        public ClassCompiler(TargetFramework framework)
            : base(AcceptedKinds)
        {
            this.framework = framework;
        }

        public ClassSymbol Compile(ClassDeclarationSyntax node, UriPath parentLink)
        {
            using Locker locker = Locker.Lock(mutex);

            string untyped = node.Identifier.ToString();
            TypeParameterListSyntax? parameterList = node.TypeParameterList;
            IEnumerable<string> parameters = parameterList is null ? [] : parameterList.Parameters.Select(p => p.Identifier.ToString());
            SymbolName name = new(untyped, parameters);

            link = parentLink.Append(name.Mangled);

            return new(name, link, framework);
        }
    }
}
