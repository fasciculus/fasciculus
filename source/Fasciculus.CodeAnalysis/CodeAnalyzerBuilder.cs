using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis
{
    public class CodeAnalyzerBuilder
    {
        private readonly CodeAnalyzerOptions options = new();

        public CodeAnalyzerBuilder WithProjectFiles(IEnumerable<FileInfo> files)
        {
            files.Apply(options.ProjectFiles.Add);

            return this;
        }

        public CodeAnalyzerBuilder WithProjectFiles(params FileInfo[] files)
            => WithProjectFiles(files.AsEnumerable());

        public CodeAnalyzerBuilder WithCombinedPackageName(string name)
        {
            options.CombinedPackageName = name;

            return this;
        }

        public CodeAnalyzerBuilder ExcludeGenerated(bool value = true)
        {
            options.IncludeGenerated = !value;

            return this;
        }

        public CodeAnalyzerBuilder IncludeNonAccessible(bool value = true)
        {
            options.IncludeNonAccessible = value;

            return this;
        }

        public CodeAnalyzerBuilder WithDebuggers(CodeAnalyzerDebuggers debuggers)
        {
            options.Debuggers = debuggers;

            return this;
        }

        public CodeAnalyzerBuilder WithNodeDebugger(INodeDebugger nodeDebugger)
        {
            options.Debuggers.NodeDebugger = nodeDebugger;

            return this;
        }

        public CodeAnalyzerBuilder WithModifierDebugger(IModifierDebugger modifierDebugger)
        {
            options.Debuggers.ModifierDebugger = modifierDebugger;

            return this;
        }

        public CodeAnalyzer Build()
            => new(options);
    }
}
