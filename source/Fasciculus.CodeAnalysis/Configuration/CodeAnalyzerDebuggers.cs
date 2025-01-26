using Fasciculus.CodeAnalysis.Debugging;

namespace Fasciculus.CodeAnalysis.Configuration
{
    public class CodeAnalyzerDebuggers
    {
        public INodeDebugger NodeDebugger { get; set; } = new NullNodeDebugger();

        public IModifierDebugger ModifierDebugger { get; set; } = new NullModifierDebugger();

        public IAccessorDebugger AccessorDebugger { get; set; } = new NullAccessorDebugger();

        public ICommentDebugger CommentDebugger { get; set; } = new NullCommentDebugger();
    }
}
