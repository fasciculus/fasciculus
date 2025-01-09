using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzerOptions
    {
        public List<FileInfo> ProjectFiles { get; set; } = [];

        public string CombinedPackageName { get; set; } = "Combined";

        public bool IncludeGenerated { get; set; } = false;

        public bool AccessibleOnly { get; set; } = true;
    }
}
