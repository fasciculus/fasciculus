using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Support
{
    public class SyntaxInfo
    {
        private readonly SortedSet<SyntaxKind> handled;
        private readonly SortedSet<SyntaxKind> used = [];

        public SyntaxInfo(IEnumerable<SyntaxKind> handled)
        {
            this.handled = new(handled);
        }

        public void AddUsed(IEnumerable<SyntaxKind> kinds)
        {
            kinds.Apply(k => { used.Add(k); });
        }

        public SortedSet<SyntaxKind> GetUnhandled()
        {
            SortedSet<SyntaxKind> unhandled = new(used);

            unhandled.ExceptWith(handled);

            return unhandled;
        }
    }

    public class Syntax
    {
        public static readonly Syntax Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<SyntaxKind, SyntaxInfo> infos = [];

        private Syntax()
        {
            AddInfos();
        }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            SyntaxKind kind = node.Kind();
            IEnumerable<SyntaxKind> used = node.ChildNodes().Select(c => c.Kind());

            if (!infos.TryGetValue(kind, out SyntaxInfo? info))
            {
                info = new([]);
                infos.Add(kind, info);
            }

            info.AddUsed(used);
        }

        public Dictionary<SyntaxKind, SortedSet<SyntaxKind>> GetUnhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            Dictionary<SyntaxKind, SortedSet<SyntaxKind>> result = [];

            foreach (var kvp in infos)
            {
                SortedSet<SyntaxKind> unhandled = kvp.Value.GetUnhandled();

                if (unhandled.Count > 0)
                {
                    result.Add(kvp.Key, unhandled);
                }
            }

            return result;
        }

        private readonly SyntaxKind[] AliasQualifiedNameKinds
            = [SyntaxKind.IdentifierName];

        private readonly SyntaxKind[] CompilationUnitKinds
            = [SyntaxKind.UsingDirective];

        private readonly SyntaxKind[] AttributeListKinds
            = [SyntaxKind.Attribute, SyntaxKind.AttributeTargetSpecifier];

        private readonly SyntaxKind[] NamespaceDeclarationKinds
            = [];

        private readonly SyntaxKind[] QualifiedNameKinds
            = [SyntaxKind.AliasQualifiedName, SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private readonly SyntaxKind[] UsingDirectiveKinds
            = [SyntaxKind.IdentifierName, SyntaxKind.QualifiedName];

        private void AddInfos()
        {
            infos.Add(SyntaxKind.AliasQualifiedName, new(AliasQualifiedNameKinds));
            infos.Add(SyntaxKind.AttributeList, new(AttributeListKinds));
            infos.Add(SyntaxKind.CompilationUnit, new(CompilationUnitKinds));
            infos.Add(SyntaxKind.NamespaceDeclaration, new(NamespaceDeclarationKinds));
            infos.Add(SyntaxKind.QualifiedName, new(QualifiedNameKinds));
            infos.Add(SyntaxKind.UsingDirective, new(UsingDirectiveKinds));
        }
    }
}
