using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal class AccessorsCompiler : CompilerBase
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly IAccessorDebugger accessorDebugger;

        private AccessorList accessors = [];

        public AccessorsCompiler(CompilerContext context)
            : base(context, SyntaxWalkerDepth.Node)
        {
            accessorDebugger = context.Debuggers.AccessorDebugger;
        }

        public AccessorList Compile(PropertyDeclarationSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            accessors = [];

            node.Accept(this);

            if (accessors.Count == 0)
            {
                accessors.Add(AccessorInfo.CreateGet(new()));
            }

            return accessors;
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            // AccessorList: GetAccessorDeclaration? SetAccessorDeclaration?
            // and more

            nodeDebugger.Add(node);

            base.VisitAccessorList(node);
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            // covers GetAccessorDeclaration, SetAccessorDeclaration, InitAccessorDeclaration, AddAccessorDeclaration,
            //  RemoveAccessorDeclaration, UnknownAccessorDeclaration
            //
            // GetAccessorDeclaration: ArrowExpressionClause?
            // SetAccessorDeclaration:

            nodeDebugger.Add(node);
            accessorDebugger.Add(node);

            SymbolModifiers modifiers = modifiersCompiler.Compile(node.Modifiers);

            if (IsIncluded(modifiers))
            {
                AccessorInfo accessor = node.Kind() switch
                {
                    SyntaxKind.GetAccessorDeclaration => AccessorInfo.CreateGet(modifiers),
                    SyntaxKind.SetAccessorDeclaration => AccessorInfo.CreateSet(modifiers),
                    _ => AccessorInfo.CreateUnknown(modifiers),
                };

                if (accessor.Kind != AccessorKind.Unknown)
                {
                    accessors.Add(accessor);
                }
            }
        }
    }
}
