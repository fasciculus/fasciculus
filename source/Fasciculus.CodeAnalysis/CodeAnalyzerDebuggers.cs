using Fasciculus.CodeAnalysis.Debugging;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzerDebuggers
    {
        public INodeDebugger NodeDebugger { get; set; } = new NullNodeDebugger();
    }
}
