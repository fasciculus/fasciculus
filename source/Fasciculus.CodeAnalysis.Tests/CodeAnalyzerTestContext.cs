using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Debugging;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Tests
{
    public class CodeAnalyzerTestContext
    {
        public required List<CodeAnalyzerProject> Projects { get; init; }

        public required SyntaxKind ProductionKind { get; init; }

        public required int Packages { get; init; }
        public required int Namespaces { get; init; }
        public required int Enums { get; init; }
        public required int Interfaces { get; init; }
        public required int Classes { get; init; }

        public required int Fields { get; init; }
        public required int Members { get; init; }
        public required int Events { get; init; }
        public required int Properties { get; init; }

        public required int Constructors { get; init; }
        public required int Methods { get; init; }

        public required int Summaries { get; init; }

        public DefaultSyntaxDebugger SyntaxDebugger { get; }
        public DefaultProductionDebugger ProductionDebugger { get; }
        public DefaultModifierDebugger ModifierDebugger { get; }
        public DefaultAccessorDebugger AccessorDebugger { get; }
        public DefaultCommentDebugger CommentDebugger { get; }

        public CodeAnalyzerDebuggers Debuggers { get; }

        public CodeAnalyzerTestContext()
        {
            SyntaxDebugger = new();
            ProductionDebugger = new(SyntaxDebugger);
            ModifierDebugger = new();
            AccessorDebugger = new();
            CommentDebugger = new();

            Debuggers = new()
            {
                NodeDebugger = ProductionDebugger,
                ModifierDebugger = ModifierDebugger,
                AccessorDebugger = AccessorDebugger,
                CommentDebugger = CommentDebugger
            };
        }
    }
}
