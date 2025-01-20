using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class AccessorsCompiler : CompilerBase
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly INodeDebugger nodeDebugger;
        private readonly IAccessorDebugger accessorDebugger;

        private AccessorList accessors = [];

        public AccessorsCompiler(CompilerContext context)
            : base(context, SyntaxWalkerDepth.Node)
        {
            nodeDebugger = context.Debuggers.NodeDebugger;
            accessorDebugger = context.Debuggers.AccessorDebugger;
        }

        public AccessorList Compile(PropertyDeclarationSyntax node)
        {
            using Locker locker = Locker.Lock(mutex);

            accessors = [];

            node.Accept(this);

            return accessors;
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            // AccessorList: GetAccessorDeclaration? SetAccessorDeclaration?

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

            base.VisitAccessorDeclaration(node);
        }

    }
}
