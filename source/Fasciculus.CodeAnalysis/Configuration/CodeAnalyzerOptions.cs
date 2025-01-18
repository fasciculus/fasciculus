using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Configuration
{
    public class CodeAnalyzerOptions
    {
        public List<CodeAnalyzerProject> Projects { get; set; } = [];

        public string CombinedPackageName { get; set; } = "Combined";

        public bool IncludeGenerated { get; set; } = false;

        public bool IncludeNonAccessible { get; set; } = false;

        public CodeAnalyzerDebuggers Debuggers { get; set; } = new();
    }
}
